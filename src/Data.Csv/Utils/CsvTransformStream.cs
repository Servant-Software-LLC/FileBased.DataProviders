using System.Diagnostics;
using System.Text;

/// <summary>
/// A custom stream wrapper that preprocesses a CSV data stream to ensure consistent column counts
/// in each row based on the header row's column count.
/// 
/// This class reads lines of text from an underlying stream (such as the BaseStream of a StreamReader),
/// checks each line for the number of commas, and appends additional commas to lines with fewer commas
/// than the header row. This ensures that all rows have the same number of columns as the header row,
/// which is especially useful for handling malformed CSV files.
/// 
/// Key Features:
/// - Reads the CSV header row during initialization to determine the expected number of columns.
/// - Transparently transforms subsequent rows as they are read, appending commas if necessary.
/// - Supports efficient, line-by-line processing for handling large streams without loading the entire
///   file into memory.
/// 
/// Usage:
/// - Wrap an existing stream (e.g., a FileStream or the BaseStream of a StreamReader) with this class.
/// - Pass this transformed stream to methods such as DataFrame.LoadCsv for CSV parsing.
/// 
/// Limitations:
/// - This class does not support seeking or writing operations.
/// - The underlying stream must contain UTF-8 encoded text data.
/// </summary>
public class CsvTransformStream : Stream
{
    private StreamReader streamReader;
    private MemoryStream bufferStream;
    private int expectedCommaCount;
    private long logicalPosition;

    public string HeaderLine { get; private set; }

    public CsvTransformStream(StreamReader streamReader)
    {
        this.streamReader = streamReader ?? throw new ArgumentNullException(nameof(streamReader));
        bufferStream = new MemoryStream();
        logicalPosition = 0;
    }

    private void InitializeHeader()
    {
        // Read the first line (header) to determine the number of commas
        HeaderLine = streamReader.ReadLine();
        if (string.IsNullOrEmpty(HeaderLine))
        {
            throw new InvalidOperationException("CSV stream is empty or header is missing.");
        }

        expectedCommaCount = HeaderLine.Split(',').Length;
        WriteToBuffer(HeaderLine + "\n");
    }

    private void WriteToBuffer(string line)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(line);
        bufferStream.Write(bytes, 0, bytes.Length);
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        // Refill the buffer with transformed lines
        bufferStream.SetLength(0);
        bufferStream.Position = 0;

        if (string.IsNullOrEmpty(HeaderLine))
        {
            //Read the header line JIT
            InitializeHeader();
        }

        if (bufferStream.Length < count)
        {
            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                int commaCount = line.Split(',').Length;
                if (commaCount < expectedCommaCount)
                {
                    line += new string(',', expectedCommaCount - commaCount);
                }

                WriteToBuffer(line + "\n");

                // Stop refilling the buffer if it has enough data to satisfy the read
                if (bufferStream.Length >= count)
                {
                    break;
                }
            }
        }

        if (bufferStream.Length == 0)
        {
            // End of the underlying stream
            return 0;
        }

        // Read from the buffer
        bufferStream.Position = 0;
        int bytesRead = bufferStream.Read(buffer, offset, count);
        logicalPosition += bytesRead; // Update logical position
        bufferStream.Position = bytesRead;

        Debug.WriteLine($"bytesRead = {bytesRead}");
        var bufferValue = Encoding.UTF8.GetString(buffer);
        Debug.WriteLine($"{bufferValue}({bufferValue.Length})");

        return bytesRead;
    }

    // Required overrides for a Stream
    public override bool CanRead => true;
    public override bool CanSeek => true;
    public override bool CanWrite => false;
    public override long Length => streamReader.BaseStream.Length;
    public override long Position
    {
        get => logicalPosition;
        set => throw new NotSupportedException();
    }
    public override void Flush() => throw new NotSupportedException();
    public override long Seek(long offset, SeekOrigin origin)
    {
        if (origin != SeekOrigin.Begin && offset != 0)
            throw new NotSupportedException();

        Debug.WriteLine("SEEK to 0");
        streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
        HeaderLine = null;

        return 0;
    }

    public override void SetLength(long value) => throw new NotSupportedException();
    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
}
