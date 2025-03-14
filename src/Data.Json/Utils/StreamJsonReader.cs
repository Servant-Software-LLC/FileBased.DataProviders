using System.Text;

namespace Data.Json.Utils;

/// <summary>
/// A streaming JSON reader that wraps a Stream and exposes a similar API to Utf8JsonReader.
/// It handles incremental buffering and paging transparently.
/// </summary>
public class StreamJsonReader
{
    private readonly Stream stream;
    private readonly int bufferSize;
    private byte[] buffer;
    private MemoryStream leftover;
    private bool isFinalBlock = false;
    private readonly JsonReaderOptions readerOptions = new JsonReaderOptions
    {
        AllowTrailingCommas = true
    };

    /// <summary>
    /// Gets the current reader state.
    /// </summary>
    public JsonReaderState CurrentState { get; private set; }

    /// <summary>
    /// Gets the type of the current JSON token.
    /// </summary>
    public JsonTokenType TokenType { get; private set; }

    /// <summary>
    /// Gets the starting index of the current token relative to the current combined buffer.
    /// </summary>
    public long TokenStartIndex { get; private set; }

    /// <summary>
    /// Gets the number of bytes consumed for the current token from the combined buffer.
    /// </summary>
    public long BytesConsumed { get; private set; }

    /// <summary>
    /// Gets the total number of bytes consumed from the stream so far.
    /// </summary>
    public long TotalBytesConsumed { get; private set; }

    public long TokenAbsoluteIndex => TotalBytesConsumed - BytesConsumed + TokenStartIndex;

    public string PropertyString { get; private set; }

    /// <summary>
    /// If set, then buffers read from the stream provided in the ctor are written to this preamble stream.
    /// </summary>
    public Stream Preamble { get; set; }

    public long LeftoverLength => leftover.Length;

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamJsonReader"/> class.
    /// </summary>
    /// <param name="stream">The underlying stream containing JSON data. Must be non-null.</param>
    /// <param name="bufferSize">The buffer size (in bytes) used for incremental reading. Default is 4096.</param>
    public StreamJsonReader(Stream stream, int bufferSize = 4096)
    {
        stream.Seek(0, SeekOrigin.Begin);
        this.stream = stream ?? throw new ArgumentNullException(nameof(stream));
        this.bufferSize = bufferSize;
        buffer = new byte[bufferSize];
        leftover = new MemoryStream();
        CurrentState = new JsonReaderState(readerOptions);
        TotalBytesConsumed = 0;
    }

    /// <summary>
    /// Attempts to read the next JSON token from the underlying stream.
    /// If <paramref name="bytesConsumedList"/> is not null, appends only the new bytes read from the stream.
    /// Returns true if a token is successfully read; false if end-of-stream is reached.
    /// </summary>
    /// <param name="bytesConsumedList">Optional list into which new bytes read are appended.</param>
    public bool Read(List<byte> bytesConsumedList = null) => Read_Internal(bytesConsumedList, false);

    private bool Read_Internal(List<byte> bytesConsumedList, bool skipGrabbingPropertyString)
    { 
        while (true)
        {
            // Create a combined buffer consisting of leftover bytes (if any) plus new bytes.
            int leftoverCount = (int)leftover.Length;

            //Do we need to replenish the leftovers?
            int bytesReadThisPass = 0;
            if (leftoverCount < bufferSize/2)
            {
                // Read from the underlying stream into our temporary buffer.
                int bytesRead = stream.Read(buffer, 0, bufferSize);
                bytesReadThisPass = bytesRead;
                if (bytesRead == 0)
                {
                    isFinalBlock = true;
                }

                if (Preamble != null)
                {
                    Preamble.Write(buffer, 0, bytesRead);
                }
            }

            byte[] combined;
            
            if (bytesReadThisPass > 0)
            {
                combined = new byte[leftoverCount + bytesReadThisPass];
                if (leftoverCount > 0)
                {
                    Array.Copy(leftover.ToArray(), combined, leftoverCount);
                }
                if (bytesReadThisPass > 0)
                {
                    Array.Copy(buffer, 0, combined, leftoverCount, bytesReadThisPass);
                }
            }
            else
            {
                // We did not add another buffer this round.
                combined = leftover.ToArray();
            }

            int combinedLength = combined.Length;

            // Create a Utf8JsonReader over the combined buffer.
            var spanCombined = combined.AsSpan(0, combinedLength);
            //string result = Encoding.UTF8.GetString(combined, 0, combinedLength);
            var reader = new Utf8JsonReader(spanCombined, isFinalBlock, CurrentState);

            if (reader.Read())
            {
                // Update our properties from the reader.
                TokenType = reader.TokenType;
                TokenStartIndex = reader.TokenStartIndex;
                BytesConsumed = reader.BytesConsumed;
                TotalBytesConsumed += BytesConsumed;
                CurrentState = reader.CurrentState;

                //Only store the ability to call GetString(), if the token is a String, PropertyName or Null
                if (!skipGrabbingPropertyString && TokenType == JsonTokenType.PropertyName)
                {
                    PropertyString = reader.GetString();
                }
                else
                {
                    PropertyString = null;
                }

                // Compute how many bytes were left unconsumed.
                int consumed = (int)reader.BytesConsumed;

                // Append only the first 'consumed' bytes from the combined buffer.
                if (bytesConsumedList != null && consumed > 0)
                {
                    for (int i = 0; i < consumed; i++)
                    {
                        bytesConsumedList.Add(combined[i]);
                    }
                }

                int remaining = combinedLength - consumed;

                // Replace leftover with the remaining bytes.
                leftover.Dispose();
                leftover = new MemoryStream();
                if (remaining > 0)
                {
                    leftover.Write(combined, consumed, remaining);
                }

                return true;
            }
            else
            {
                if (isFinalBlock)
                    return false;
                // Otherwise, continue reading additional bytes.
            }
        }
    }

    /// <summary>
    /// Attempts to skip the current JSON value (if it is a container) so that the reader advances past it.
    /// This method returns true if the value was successfully skipped, or false if the end of the stream was reached
    /// before the entire value could be skipped.
    /// </summary>
    /// <param name="bytesConsumedList">Optional list into which new bytes read while skipping are appended.</param>
    public bool TrySkip(List<byte> bytesConsumedList = null)
    {
        // If the current token is not a container, nothing to skip.
        if (TokenType != JsonTokenType.StartObject && TokenType != JsonTokenType.StartArray)
            throw new Exception($"Cannot skip because token type was not an object nor an array. TokenType = {TokenType}");

        int depth = 1;
        while (depth > 0)
        {
            // Attempt to read the next token.
            if (!Read_Internal(bytesConsumedList, true))
                return false; // End-of-stream encountered before fully skipping.

            if (TokenType == JsonTokenType.StartObject || TokenType == JsonTokenType.StartArray)
                depth++;
            else if (TokenType == JsonTokenType.EndObject || TokenType == JsonTokenType.EndArray)
                depth--;
        }
        return true;
    }

    /// <summary>
    /// Parses the current JSON container (object or array) into a JsonDocument.
    /// This method creates an accumulator list, passes it to TrySkip so that only the bytes consumed
    /// during the skipping of the container are accumulated, and then parses those bytes into a JsonDocument.
    /// </summary>
    public JsonDocument ParseCurrentValue()
    {
        if (TokenType != JsonTokenType.StartObject && TokenType != JsonTokenType.StartArray)
            throw new InvalidOperationException("Current token is not a container.");

        List<byte> bytesConsumedList = new List<byte>() { (byte)(TokenType == JsonTokenType.StartObject ? '{' : '[') };

        if (!TrySkip(bytesConsumedList))
            throw new Exception("Failed to skip container.");

        byte[] data = bytesConsumedList.ToArray();
        var dataStr = Encoding.UTF8.GetString(data, 0, bytesConsumedList.Count);
        JsonDocumentOptions jsonDocumentOptions = new() { AllowTrailingCommas = true };
        return JsonDocument.Parse(data, jsonDocumentOptions);
    }

    /// <summary>
    /// Returns the string value of the current property token.
    /// </summary>
    /// <remarks>
    /// This method does NOT following the same behavior as Utf8JsonReader.GetString.  To optimize performance, the string of 
    /// the property token is only grabbed when we're not skipping.  If the token was String or Null, we don't grab that.
    /// </remarks>
    public string GetString() => PropertyString != null ? PropertyString : throw new InvalidOperationException($"Unable to GetString() because the current token isn't JsonTokenType.PropertyName.  TokenType: {TokenType}");
}
