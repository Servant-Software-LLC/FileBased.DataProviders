using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

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
        var rawHeaderLine = streamReader.ReadLine();
        if (string.IsNullOrEmpty(rawHeaderLine))
        {
            throw new InvalidOperationException("CSV stream is empty or header is missing.");
        }

        // Replace all whitespace (including non-breaking) with a single space
        HeaderLine = Regex.Replace(rawHeaderLine, @"\s+", " ").Replace("\uFFFD", "");

        expectedCommaCount = HeaderLine.Split(',').Length - 1;
        WriteToBuffer(HeaderLine + "\n");
    }

    private void WriteToBuffer(string line)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(line);
        bufferStream.Write(bytes, 0, bytes.Length);
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        //TODO: Deal with an offset, only if we find it is required.
        if (offset > 0)
            throw new NotSupportedException($"Didn't expect a buffer with an offset greater than zero.  Offset: {offset}");

        if (string.IsNullOrEmpty(HeaderLine))
        {
            //Read the header line JIT
            InitializeHeader();
        }

        //Check if the header is larger than 1024 or if a line is greater than twice 1024. 
        int bytesRead = StreamFillsCompleteBuffer(buffer, count);
        if (bytesRead > 0)
        {
            return bytesRead;
        }

        if (bufferStream.Length < count)
        {
            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                int commaCount = CountCommasOutsideQuotes(line);
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

        bytesRead = StreamFillsCompleteBuffer(buffer, count);
        if (bytesRead > 0)
        {
            Debug.WriteLine($"{Encoding.UTF8.GetString(buffer)}");
            Debug.WriteLine($"BUFFER READ({bytesRead})");
            return bytesRead;
        }

        // Read from the buffer.  This is the last of the stream.
        bufferStream.Position = 0;
        bytesRead = bufferStream.Read(buffer, 0, (int)bufferStream.Length);
        logicalPosition += bytesRead; // Update logical position
        bufferStream = new MemoryStream();

        Debug.WriteLine($"{Encoding.UTF8.GetString(buffer.Take(bytesRead).ToArray())}");
        Debug.WriteLine($"END READ({bytesRead})");
        return bytesRead;
    }

    private int CountCommasOutsideQuotes(string line)
    {
        bool inQuotes = false;
        int commaCount = 0;

        foreach (char c in line)
        {
            if (c == '"') inQuotes = !inQuotes;
            else if (c == ',' && !inQuotes) commaCount++;
        }

        return commaCount;
    }

    /// <summary>
    /// Checks to see if a line pushed on to the streamBuffer is larger than the buffer.
    /// For instance, DataFrame uses a count of 1024. But what if the header line was bigger than 1024, in that 
    /// case we need to deliver only part of it.
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="count"></param>
    /// <returns>Bytes read or 0 if stream buffer is smaller than count</returns>
    private int StreamFillsCompleteBuffer(byte[] buffer, int count)
    {
        if (bufferStream.Length >= count)
        {
            //The stream is bigger than the buffer, so we can't hand back complete lines. Hand back
            //what we can.
            bufferStream.Position = 0;
            int bytesRead = bufferStream.Read(buffer, 0, count);

            ReduceStreamBuffer(count);
            return bytesRead;
        }

        return 0;
    }

    private void ReduceStreamBuffer(int count)
    {
        var remainingNumberBytes = (int)bufferStream.Length - count;
        var fullBufferStream = bufferStream.ToArray();
        var remainingBytes = fullBufferStream.Skip(count).ToArray();
        var remainingBuffer = Encoding.UTF8.GetString(remainingBytes);
        bufferStream = new MemoryStream();
        WriteToBuffer(remainingBuffer);
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

        Debug.WriteLine("SEEK 0");
        streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
        streamReader.DiscardBufferedData();
        HeaderLine = null;
        bufferStream = new MemoryStream();

        return 0;
    }

    public override void SetLength(long value) => throw new NotSupportedException();
    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
}
