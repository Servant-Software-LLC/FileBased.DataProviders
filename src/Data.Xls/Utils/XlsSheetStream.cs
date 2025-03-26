using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Text;

namespace Data.Xls.Utils;

/// <summary>
/// A transformation stream that, when read, produces CSV-formatted rows for an XLS/XLSX sheet.
/// This implementation uses the SAX approach (via OpenXmlReader) to process the sheet's XML
/// without loading the entire sheet into memory.
/// </summary>
public class XlsSheetStream : Stream, IDisposable
{
    private readonly WorksheetPart worksheetPart;
    private readonly WorkbookPart workbookPart;
    private IEnumerator<string> csvLineEnumerator;
    private MemoryStream currentBuffer;
    private bool disposed = false;
    private Stream _stream;

    /// <summary>
    /// Initializes a new instance of the <see cref="XlsSheetStream"/> class.
    /// </summary>
    /// <param name="worksheetPart">The WorksheetPart representing the sheet to be transformed.</param>
    public XlsSheetStream(Stream stream, WorkbookPart workbookPart, WorksheetPart worksheetPart)
    {
        _stream = stream;
        this.worksheetPart = worksheetPart ?? throw new ArgumentNullException(nameof(worksheetPart));
        this.workbookPart = workbookPart ?? throw new ArgumentNullException(nameof(workbookPart));
        csvLineEnumerator = CreateCsvLineEnumerator(worksheetPart);
        currentBuffer = new MemoryStream();
    }

    /// <summary>
    /// Creates an enumerator that uses OpenXmlReader (SAX) to stream through the sheet’s XML,
    /// converting each Row element into a CSV-formatted line.
    /// </summary>
    private IEnumerator<string> CreateCsvLineEnumerator(WorksheetPart wsPart)
    {
        // Use OpenXmlReader for forward-only, low-memory parsing.
        using (var reader = OpenXmlReader.Create(wsPart))
        {
            var maxColumnCount = 0;
            // Loop through the XML elements.
            while (reader.Read())
            {
                if (reader.ElementType == typeof(Row))
                {
                    // Load the row element (we can use LoadCurrentElement because a single row should be small).
                    Row row = (Row)reader.LoadCurrentElement();
                    // Build a CSV line by iterating through cells.
                    List<string> cells = new List<string>();

                    var expectedColumnIndex = 0; // Track expected column index
                    foreach (Cell cell in row.Elements<Cell>())
                    {
                        var currentColumnIndex = cell.GetColumnIndex(); // Convert A1 notation to index

                        // Fill in missing columns with empty strings
                        while (expectedColumnIndex < currentColumnIndex)
                        {
                            cells.Add("");
                            expectedColumnIndex++;
                        }

                        // Process the actual cell value
                        cells.Add(workbookPart.GetCellValue(cell));
                        expectedColumnIndex++;
                    }
                    
                    maxColumnCount = Math.Max(maxColumnCount, expectedColumnIndex);
                    
                    // If the last columns were missing, fill them with empty values
                    while (cells.Count < maxColumnCount)
                    {
                        cells.Add("");
                    }

                    yield return string.Join(",", cells.EscapeCsvValues());
                }
            }
        }
    }

    /// <summary>
    /// Reads data from the stream. The stream outputs CSV-formatted text, one row per line.
    /// This implementation reads from an internal buffer that is filled on demand from the CSV line enumerator.
    /// </summary>
    public override int Read(byte[] outBuffer, int offset, int count)
    {
        if (disposed)
            throw new ObjectDisposedException(nameof(XlsSheetStream));

        int totalRead = 0;
        while (totalRead < count)
        {
            // If currentBuffer has data, read from it.
            if (currentBuffer.Position < currentBuffer.Length)
            {
                int n = currentBuffer.Read(outBuffer, offset + totalRead, count - totalRead);
                totalRead += n;
            }
            else
            {
                // If currentBuffer is exhausted, try to get the next CSV line.
                if (csvLineEnumerator.MoveNext())
                {
                    // Clear the buffer and load the next CSV line.
                    currentBuffer.SetLength(0);
                    currentBuffer.Position = 0;
                    string line = csvLineEnumerator.Current;
                    byte[] lineBytes = Encoding.UTF8.GetBytes(line + Environment.NewLine);
                    currentBuffer.Write(lineBytes, 0, lineBytes.Length);
                    currentBuffer.Position = 0;
                }
                else
                {
                    // No more lines available.
                    break;
                }
            }
        }

        return totalRead;
    }

    public override bool CanRead => true;
    public override bool CanSeek => false;
    public override bool CanWrite => false;
    public override long Length => throw new NotSupportedException();

    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    public override void Flush()
    {
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        if (origin != SeekOrigin.Begin && offset != 0)
            throw new NotSupportedException();

        _stream.Seek(0, origin);
        currentBuffer = new MemoryStream();
        csvLineEnumerator = CreateCsvLineEnumerator(worksheetPart);
        return 0;
    }

    public override void SetLength(long value) => throw new NotSupportedException();
    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    public new void Dispose()
    {
        if (!disposed)
        {
            currentBuffer.Dispose();
            csvLineEnumerator.Dispose();
            base.Dispose();
            disposed = true;
        }
    }
}