using Data.Tests.Common.Utils;
using System.Text;
using System.Text.Json;

namespace Data.Json.Tests.Utils;

/// <summary>
/// A stream that produces JSON data on demand from TRecord instances.
/// The unit test creator provides a delegate to create new TRecord values,
/// so that the test can steer the data and later assert on it.
/// This stream outputs a JSON array that begins with an opening bracket.
/// Each record is serialized using indented formatting (and may span multiple lines),
/// with a preceding comma for every record after the first.
/// Note: No closing bracket is written.
/// </summary>
internal class UnendingJsonStream<TRecord> : UnendingStream, IDisposable
{
    private readonly Func<TRecord> createObject;
    private readonly MemoryStream memoryStream;
    private readonly StreamWriter streamWriter;
    private readonly JsonSerializerOptions jsonOptions;
    private bool disposed = false;
    private bool firstRecord = true;

    public UnendingJsonStream(Func<TRecord> createObject)
    {
        this.createObject = createObject ?? throw new ArgumentNullException(nameof(createObject));
        memoryStream = new MemoryStream();
        streamWriter = new StreamWriter(memoryStream, Encoding.UTF8, bufferSize: 1024, leaveOpen: true);
        jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        // Write the opening bracket for the JSON array.
        streamWriter.WriteLine("[");
        streamWriter.Flush();
    }

    /// <summary>
    /// Produces additional JSON bytes by serializing a TRecord instance.
    /// For the first record, no comma is added; for subsequent records, a comma and newline are prepended.
    /// </summary>
    protected override byte[] GetMoreBytes()
    {
        if (firstRecord)
        {
            firstRecord = false;
            // For the first record, simply serialize and write.
            TRecord record = createObject();
            string json = JsonSerializer.Serialize(record, jsonOptions);
            streamWriter.Write(json);
            streamWriter.WriteLine();
        }
        else
        {
            // For subsequent records, prepend a comma and newline.
            streamWriter.WriteLine(",");
            TRecord record = createObject();
            string json = JsonSerializer.Serialize(record, jsonOptions);
            streamWriter.Write(json);
            streamWriter.WriteLine();
        }
        streamWriter.Flush();

        // Capture the newly written bytes.
        byte[] bytes = memoryStream.ToArray();

        // Clear the memory stream for the next record.
        memoryStream.SetLength(0);
        memoryStream.Position = 0;

        return bytes;
    }

    protected override void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                streamWriter?.Dispose();
                memoryStream?.Dispose();
            }
            base.Dispose(disposing);
            disposed = true;
        }
    }

    void IDisposable.Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
