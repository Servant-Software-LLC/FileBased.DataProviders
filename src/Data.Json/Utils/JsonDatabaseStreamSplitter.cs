using Data.Common.Utils;

namespace Data.Json.Utils;

/// <summary>
/// Splits a JSON document (whose root is an object mapping table names to arrays)
/// into substreams—one for each table. Each substream reads only the portion of the underlying
/// stream that represents that table's JSON array.
/// 
/// This class assumes that the underlying stream is seekable and that the JSON structure is
/// well-formed.
/// </summary>
public class JsonDatabaseStreamSplitter : IDisposable
{
    private readonly Stream baseStream;
    private readonly int bufferSize;

    public JsonDatabaseStreamSplitter(Stream baseStream, int bufferSize = 4096)
    {
        this.baseStream = baseStream ?? throw new ArgumentNullException(nameof(baseStream));
        if (!baseStream.CanSeek)
            throw new ArgumentException("The base stream must be seekable.", nameof(baseStream));
        this.bufferSize = bufferSize;
    }

    /// <summary>
    /// Reads the JSON document and returns a dictionary mapping each table name (property name)
    /// to a stream that covers the JSON array for that table.
    /// </summary>
    /// <returns>A dictionary of table names and their corresponding substreams.</returns>
    public IDictionary<string, Stream> GetTableStreams()
    {
        var result = new Dictionary<string, Stream>(StringComparer.OrdinalIgnoreCase);
        // Dictionary to hold table positions: tableName -> (start offset, end offset)
        var tablePositions = new Dictionary<string, (long start, long end)>(StringComparer.OrdinalIgnoreCase);

        // Save the original position.
        long originalPosition = baseStream.Position;
        baseStream.Seek(0, SeekOrigin.Begin);

        bool insideRoot = false;
        string currentProperty = null;
        long arrayStart = 0;

        // We use a loop to read through the file in chunks.
        var reader = new StreamJsonReader(baseStream, true);
        while (reader.Read())
        {
            if (!insideRoot)
            {
                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    insideRoot = true;
                }
                continue;
            }

            // The root token has already been read.

            // If we've found the table name
            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                currentProperty = reader.GetString();
                continue;
            }

            // If we're at the table data
            if (reader.TokenType == JsonTokenType.StartArray && currentProperty != null)
            {
                // Record the start offset of the array.
                arrayStart = reader.TokenAbsoluteIndex;

                //Skip this entire array.
                var success = reader.TrySkip();
                if (!success)
                    throw new Exception("The end of the stream was encountered before skipping this JSON array.");

                long arrayEnd = reader.TokenAbsoluteIndex + 1;
                tablePositions[currentProperty] = (arrayStart, arrayEnd);
                currentProperty = null;
                continue;
            }

            // If we're at the end of the root
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                //Nothing more to do.
                continue;
            }

            throw new DataException($"Expected JSON token.  Token type: {reader.TokenType}  currentProperty: {currentProperty}");
        }

        // Create a substream for each table.
        foreach (var kvp in tablePositions)
        {
            long start = kvp.Value.start;
            long end = kvp.Value.end;
            long length = end - start;
            result[kvp.Key] = new SubStream(baseStream, start, length);
        }

        // Restore the original position.
        baseStream.Seek(originalPosition, SeekOrigin.Begin);

        return result;
    }

    public void Dispose()
    {
        // Note: We don't dispose baseStream here because the SubStreams are still using it
        // The baseStream should be disposed by the caller after all SubStreams are done
        // baseStream.Dispose();
    }
}
