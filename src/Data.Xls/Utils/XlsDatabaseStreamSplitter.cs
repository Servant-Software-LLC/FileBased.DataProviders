using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Data.Xls.Utils;

/// <summary>
/// Splits an XLS/XLSX file (opened via the Open XML SDK) into substreams—one per sheet.
/// Each substream is an XlsSheetStream that, when read, produces CSV‐formatted rows
/// representing that sheet’s data.
/// 
/// This implementation uses the SAX (OpenXmlReader) approach so that very large files
/// are processed without loading the entire document into memory.
/// </summary>
public class XlsDatabaseStreamSplitter : IDisposable
{
    private readonly SpreadsheetDocument document;
    private bool disposed = false;
    private Stream _stream;

    /// <summary>
    /// Initializes a new instance of the <see cref="XlsDatabaseStreamSplitter"/> class using the provided XLS/XLSX stream.
    /// </summary>
    /// <param name="stream">A stream containing an XLS or XLSX file.</param>
    public XlsDatabaseStreamSplitter(Stream stream)
    {
        _stream = stream;
        if (_stream == null) throw new ArgumentNullException(nameof(_stream));
        // Open the document in read-only mode.
        document = SpreadsheetDocument.Open(_stream, false);
    }

    /// <summary>
    /// Returns a dictionary mapping each sheet name to a stream that transforms that sheet's data into CSV-formatted rows.
    /// </summary>
    /// <returns>A dictionary of sheet names and corresponding XlsSheetStream instances.</returns>
    public IDictionary<string, Stream> GetTableStreams()
    {
        var result = new Dictionary<string, Stream>(StringComparer.OrdinalIgnoreCase);
        var workbookPart = document.WorkbookPart;
        var sheets = workbookPart.Workbook.Sheets;
        foreach (Sheet sheet in sheets.Elements<Sheet>())
        {
            // Retrieve the corresponding WorksheetPart.
            WorksheetPart worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id);
            // Create an XlsSheetStream for this sheet (using the SAX approach).
            var sheetStream = new XlsSheetStream(_stream, worksheetPart);
            result[sheet.Name] = sheetStream;
        }
        return result;
    }

    public void Dispose()
    {
        if (!disposed)
        {
            document.Dispose();
            disposed = true;
        }
    }
}
