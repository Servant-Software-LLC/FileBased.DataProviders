using CsvHelper;
using Data.Tests.Common.Utils;
using System.Globalization;
using System.Text;

namespace Data.Csv.Tests.Utils;

/// <summary>
/// A stream that produces CSV data on demand from TRecord instances.
/// The unit test creator provides a delegate to create new TRecord values,
/// so that the test can steer the data and later assert on it.
/// </summary>
internal class UnendingCsvStream<TRecord> : UnendingStream, IDisposable
{
    private readonly Func<TRecord> createObject;
    private readonly MemoryStream memoryStream;
    private readonly StreamWriter streamWriter;
    private readonly CsvWriter csvWriter;
    private bool disposed = false;

    public UnendingCsvStream(Func<TRecord> createObject)
    {
        this.createObject = createObject ?? throw new ArgumentNullException(nameof(createObject));  
        memoryStream = new MemoryStream();
        streamWriter = new StreamWriter(memoryStream, Encoding.UTF8, bufferSize: 1024, leaveOpen: true);
        csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);

        //Write the header.
        csvWriter.WriteHeader<TRecord>();
        csvWriter.NextRecord();
        csvWriter.Flush();
    }

    protected override byte[] GetMoreBytes()
    {
        // Create a new record instance.
        TRecord record = createObject();

        // Write the record and move to the next line.
        csvWriter.WriteRecord(record);
        csvWriter.NextRecord();

        // Ensure all data is flushed to the MemoryStream.
        streamWriter.Flush();

        // Get the newly written bytes.
        byte[] bytes = memoryStream.ToArray();

        // Reset the MemoryStream for the next write.
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
                csvWriter?.Dispose();
                streamWriter?.Dispose();
                memoryStream?.Dispose();
            }

            // Call the base class Dispose method to allow it to clean up its resources.
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
