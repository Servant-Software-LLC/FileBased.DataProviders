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
    private int bufferPosition = 0;
    private int bufferLength = 0;
    private bool isFinalBlock = false;
    private readonly JsonReaderOptions readerOptions = new JsonReaderOptions
    {
        AllowTrailingCommas = true,
        CommentHandling = JsonCommentHandling.Skip
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

    public long LeftoverLength => bufferLength - bufferPosition;

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamJsonReader"/> class.
    /// </summary>
    /// <param name="stream">The underlying stream containing JSON data. Must be non-null.</param>
    /// <param name="bufferSize">The buffer size (in bytes) used for incremental reading. Default is 4096.</param>
    public StreamJsonReader(Stream stream, bool considerBOM, int bufferSize = 4096)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        // Ensure we're at the beginning of the stream
        stream.Seek(0, SeekOrigin.Begin);

        this.stream = considerBOM ? new BOMHandlingStream(stream) : stream;
        this.bufferSize = bufferSize;
        buffer = new byte[bufferSize];
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
            // Check if we need to read more data
            bool dataRead = EnsureDataAvailable();

            // If we couldn't read any data and we're at the end of our buffer
            if (!dataRead && bufferPosition >= bufferLength && isFinalBlock)
            {
                return false;
            }

            // Create a span over the remaining buffer data
            var spanCombined = new ReadOnlySpan<byte>(buffer, bufferPosition, bufferLength - bufferPosition);
            
            //NOTE: Used for debugging.
            //string result = Encoding.UTF8.GetString(spanCombined);
            
            var reader = new Utf8JsonReader(spanCombined, isFinalBlock, CurrentState);

            try
            {
                if (reader.Read())
                {
                    // Update our properties from the reader.
                    TokenType = reader.TokenType;
                    TokenStartIndex = reader.TokenStartIndex;
                    BytesConsumed = reader.BytesConsumed;
                    
                    // If we need to collect the bytes for the caller
                    if (bytesConsumedList != null && BytesConsumed > 0)
                    {
                        for (int i = 0; i < BytesConsumed; i++)
                        {
                            bytesConsumedList.Add(buffer[bufferPosition + i]);
                        }
                    }

                    // Increment total bytes consumed
                    TotalBytesConsumed += BytesConsumed;
                    
                    // Update our buffer position to skip the consumed bytes
                    bufferPosition += (int)BytesConsumed;
                    
                    // Store the current state
                    CurrentState = reader.CurrentState;

                    // Only store the property string if needed
                    if (!skipGrabbingPropertyString && TokenType == JsonTokenType.PropertyName)
                    {
                        PropertyString = reader.GetString();
                    }
                    else
                    {
                        PropertyString = null;
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
            catch (System.Text.Json.JsonException)
            {
                // Special handling for partial BOMs and invalid JSON starts:
                // If we have data in the buffer but can't read it, skip the first byte and try again
                if (bufferPosition < bufferLength)
                {
                    // Skip the first byte (which might be part of a partial BOM)
                    bufferPosition++;
                    
                    // If we've skipped everything and have no more data, return false
                    if (bufferPosition >= bufferLength && isFinalBlock)
                    {
                        return false;
                    }
                    
                    // Continue to the next iteration and try to read again
                    continue;
                }
                
                // If we have no data left, return false
                if (isFinalBlock)
                    return false;
            }
        }
    }

    /// <summary>
    /// Ensures there is data available in the buffer to be read.
    /// Returns true if new data was read from the stream.
    /// </summary>
    private bool EnsureDataAvailable()
    {
        // If we still have sufficient data in the buffer, no need to read more
        if (bufferPosition < bufferLength && (bufferLength - bufferPosition) > bufferSize / 2)
        {
            return false;
        }

        // If we've consumed part of the buffer, compact it
        if (bufferPosition > 0)
        {
            if (bufferPosition < bufferLength)
            {
                // Move remaining data to the start of the buffer
                Buffer.BlockCopy(buffer, bufferPosition, buffer, 0, bufferLength - bufferPosition);
            }
            
            // Adjust length to account for the compaction
            bufferLength -= bufferPosition;
            bufferPosition = 0;
        }

        // If our buffer is too small, resize it
        if (buffer.Length - bufferLength < bufferSize)
        {
            byte[] newBuffer = new byte[buffer.Length * 2];
            Buffer.BlockCopy(buffer, 0, newBuffer, 0, bufferLength);
            buffer = newBuffer;
        }

        // Read new data into the buffer at the current position
        int bytesRead = stream.Read(buffer, bufferLength, buffer.Length - bufferLength);
        
        if (bytesRead == 0)
        {
            isFinalBlock = true;
            return false;
        }

        // If we have a preamble stream, write the new bytes to it
        if (Preamble != null)
        {
            Preamble.Write(buffer, bufferLength, bytesRead);
        }

        // Update buffer length to include the new bytes
        bufferLength += bytesRead;
        return true;
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
        int maxIterations = 100000; // Add a safety limit to prevent infinite loops
        int iterations = 0;

        while (depth > 0 && iterations < maxIterations)
        {
            iterations++;

            try
            {
                // Attempt to read the next token.
                if (!Read_Internal(bytesConsumedList, true))
                    return false; // End-of-stream encountered before fully skipping.

                if (TokenType == JsonTokenType.StartObject || TokenType == JsonTokenType.StartArray)
                    depth++;
                else if (TokenType == JsonTokenType.EndObject || TokenType == JsonTokenType.EndArray)
                    depth--;
            }
            catch (Exception)
            {
                // If we encounter an exception while trying to skip,
                // add the rest of the buffer to bytesConsumedList
                // and return true to indicate we "skipped" as much as possible
                if (bytesConsumedList != null && bufferLength > bufferPosition)
                {
                    for (int i = bufferPosition; i < bufferLength; i++)
                    {
                        bytesConsumedList.Add(buffer[i]);
                    }

                    // Clear the buffer
                    bufferPosition = bufferLength;
                }

                return false;
            }
        }

        // If we hit the iteration limit, clear buffer and continue
        if (iterations >= maxIterations)
        {
            bufferPosition = bufferLength;
            return false;
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

        bool skipped = TrySkip(bytesConsumedList);
        if (!skipped)
        {
            // If we couldn't skip properly, add a closing bracket/brace to make valid JSON
            bytesConsumedList.Add((byte)(TokenType == JsonTokenType.StartObject ? '}' : ']'));
        }

        try
        {
            byte[] data = bytesConsumedList.ToArray();
            var dataStr = Encoding.UTF8.GetString(data, 0, bytesConsumedList.Count);
            JsonDocumentOptions jsonDocumentOptions = new() { AllowTrailingCommas = true, CommentHandling = JsonCommentHandling.Skip };
            return JsonDocument.Parse(data, jsonDocumentOptions);
        }
        catch (Exception)
        {
            // If parsing still fails, create a minimal valid JSON object/array
            byte[] minimalJson = (TokenType == JsonTokenType.StartObject)
                ? Encoding.UTF8.GetBytes("{}")
                : Encoding.UTF8.GetBytes("[]");

            JsonDocumentOptions jsonDocumentOptions = new() { AllowTrailingCommas = true, CommentHandling = JsonCommentHandling.Skip };
            return JsonDocument.Parse(minimalJson, jsonDocumentOptions);
        }
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
