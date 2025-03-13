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
    private long bomLength = 0;
    private readonly JsonReaderOptions readerOptions = new JsonReaderOptions
    {
        AllowTrailingCommas = true
    };

    // Holds the most recent combined buffer.
    private byte[] currentCombinedBuffer = Array.Empty<byte>();

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

    /// <summary>
    /// If set, then buffers read from the stream provided in the ctor are written to this preamble stream.
    /// </summary>
    public Stream Preamble { get; set; }


    /// <summary>
    /// Initializes a new instance of the <see cref="StreamJsonReader"/> class.
    /// </summary>
    /// <param name="stream">The underlying stream containing JSON data. Must be non-null.</param>
    /// <param name="bufferSize">The buffer size (in bytes) used for incremental reading. Default is 4096.</param>
    public StreamJsonReader(Stream stream, bool checkForBom, int bufferSize = 4096)
    {
        stream.Seek(0, SeekOrigin.Begin);
        if (checkForBom)
        {
            //Check for BOM
            byte[] bomBuffer = new byte[3];
            var read = stream.Read(bomBuffer, 0, bomBuffer.Length);
            for (int counter = 0; counter < bomBuffer.Length; counter++)
            {
                if (bomBuffer[counter] > 127)
                    bomLength++;
                else
                    break;
            }
            stream.Seek(bomLength, SeekOrigin.Begin);
        }

        this.stream = stream ?? throw new ArgumentNullException(nameof(stream));
        this.bufferSize = bufferSize;
        buffer = new byte[bufferSize];
        leftover = new MemoryStream();
        CurrentState = new JsonReaderState(readerOptions);
        TotalBytesConsumed = 0;
    }

    /// <summary>
    /// Attempts to read the next JSON token from the underlying stream.
    /// Returns true if a token is successfully read; false if end-of-stream is reached.
    /// </summary>
    public bool Read()
    {
        while (true)
        {
            // Read from the underlying stream into our temporary buffer.
            int bytesRead = stream.Read(buffer, 0, bufferSize);
            if (bytesRead == 0)
            {
                isFinalBlock = true;
            }

            if (Preamble != null)
            {
                Preamble.Write(buffer, 0, bytesRead);
            }

            // Create a combined buffer consisting of leftover bytes (if any) plus new bytes.
            int leftoverCount = (int)leftover.Length;

            byte[] combined = new byte[leftoverCount + bytesRead];
            if (leftoverCount > 0)
            {
                Array.Copy(leftover.ToArray(), combined, leftoverCount);
            }
            if (bytesRead > 0)
            {
                Array.Copy(buffer, 0, combined, leftoverCount, bytesRead);
            }
            currentCombinedBuffer = combined; // Store the current combined buffer.
            int combinedLength = combined.Length;

            // Create a Utf8JsonReader over the combined buffer.
            var spanCombined = combined.AsSpan(0, combinedLength);
            string result = Encoding.UTF8.GetString(combined, 0, combinedLength);
            var reader = new Utf8JsonReader(spanCombined, isFinalBlock, CurrentState);

            if (reader.Read())
            {
                // Update our properties from the reader.
                TokenType = reader.TokenType;
                TokenStartIndex = reader.TokenStartIndex;
                BytesConsumed = reader.BytesConsumed;
                TotalBytesConsumed += BytesConsumed;
                CurrentState = reader.CurrentState;

                // Compute how many bytes were left unconsumed.
                int consumed = (int)reader.BytesConsumed;
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
    public bool TrySkip()
    {
        // If the current token is not a container, nothing to skip.
        if (TokenType != JsonTokenType.StartObject && TokenType != JsonTokenType.StartArray)
            throw new Exception($"Cannot skip because token type was not an object nor an array. TokenType = {TokenType}");

        int depth = 1;
        while (depth > 0)
        {
            // Attempt to read the next token.
            if (!Read())
                return false; // End-of-stream encountered before fully skipping.

            if (TokenType == JsonTokenType.StartObject || TokenType == JsonTokenType.StartArray)
                depth++;
            else if (TokenType == JsonTokenType.EndObject || TokenType == JsonTokenType.EndArray)
                depth--;
        }
        return true;
    }

    /// <summary>
    /// Parses the current JSON value (which must be a container) into a JsonDocument.
    /// This method behaves similarly to JsonDocument.ParseValue(ref reader).
    /// It calculates the absolute byte offsets of the container in the underlying stream,
    /// reads that segment (using the stream's seek capability), and returns a JsonDocument for that segment.
    /// </summary>
    /// <returns>A JsonDocument representing the current JSON container.</returns>
    public JsonDocument ParseCurrentValue()
    {
        if (TokenType != JsonTokenType.StartObject && TokenType != JsonTokenType.StartArray)
            throw new InvalidOperationException("Current token is not a container.");

        // Calculate the absolute start offset of the container.
        long absoluteStart = TokenAbsoluteIndex;

        // Use TrySkip to skip the container; after this, the next token's start is the container's end.
        if (!TrySkip())
            throw new Exception("Failed to skip container.");

        // Now the new TokenStartIndex should be at the end of the container.
        long absoluteEnd = TokenAbsoluteIndex + 1;
        long length = absoluteEnd - absoluteStart;
        if (length <= 0)
            throw new Exception("Container length is non-positive.");

        // Because our stream is seekable, we can now read that segment.
        long originalPosition = stream.Position;
        stream.Seek(bomLength + absoluteStart, SeekOrigin.Begin);
        byte[] data = new byte[length];
        int totalRead = 0;
        while (totalRead < length)
        {
            int read = stream.Read(data, totalRead, (int)(length - totalRead));
            if (read == 0)
                break;
            totalRead += read;
        }
        // Restore original position.
        stream.Seek(originalPosition, SeekOrigin.Begin);

        var dataStr = Encoding.UTF8.GetString(data, 0, totalRead);
        JsonDocumentOptions jsonDocumentOptions = new() { AllowTrailingCommas = true };
        return JsonDocument.Parse(data, jsonDocumentOptions);
    }

    /// <summary>
    /// Returns the string value of the current token.
    /// This method mimics Utf8JsonReader.GetString() by re-parsing the token span.
    /// </summary>
    public string GetString()
    {
        // Calculate the absolute token span within the current combined buffer.
        int start = (int)TokenStartIndex;
        int length = (int)(BytesConsumed - TokenStartIndex);
        ReadOnlySpan<byte> tokenSpan = currentCombinedBuffer.AsSpan(start, length);
        // Create a temporary reader over the token span.
        var tempReader = new Utf8JsonReader(tokenSpan, isFinalBlock: true, state: CurrentState);
        tempReader.Read();
        return tempReader.GetString();
    }
}
