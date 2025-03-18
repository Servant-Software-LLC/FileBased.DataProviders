using Data.Common.Utils.ConnectionString;
using Data.Csv.Utils;
using SqlBuildingBlocks.POCOs;
using System.Text;

namespace Data.Xls.Utils;

/// <summary>
/// A VirtualDataSet implementation that builds tables from an XLS/XLSX file.
/// The XLS file may contain many sheets; each sheet is transformed on‑the‑fly into CSV formatted rows
/// using an XlsSheetStream (which uses the SAX approach via OpenXmlReader). Each sheet is then wrapped by a
/// CsvVirtualDataTable to produce a VirtualDataTable for that sheet.
/// </summary>
public class XlsDatabaseVirtualDataSet : VirtualDataSet, IDisposable, IFreeStreams
{
    private readonly XlsDatabaseStreamSplitter splitter;
    private bool disposed = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="XlsDatabaseVirtualDataSet"/> class using the provided XLS stream splitter.
    /// </summary>
    /// <param name="splitter">
    /// A <see cref="XlsDatabaseStreamSplitter"/> that returns substreams (one per sheet) for a large XLS/XLSX file.
    /// </param>
    /// <param name="guessRows">The number of rows to use for type inference when converting to CSV.</param>
    /// <param name="pageSize">The number of CSV rows to page in at a time.</param>
    /// <param name="preferredFloatingPointDataType">The preferred floating point type for numeric columns.</param>
    /// <param name="guessTypeFunction">
    /// A delegate that takes a collection of string values and returns a <see cref="Type"/>.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown if splitter is null.</exception>
    public XlsDatabaseVirtualDataSet(
        Stream xlsStream,
        int guessRows,
        int pageSize,
        FloatingPointDataType preferredFloatingPointDataType,
        Func<IEnumerable<string>, Type> guessTypeFunction)
    {
        this.splitter = new XlsDatabaseStreamSplitter(xlsStream);
        // Get the sheet streams from the splitter.
        IDictionary<string, Stream> tableStreams = splitter.GetTableStreams();
        foreach (var kvp in tableStreams)
        {
            // kvp.Key is the sheet name, kvp.Value is an XlsSheetStream that produces CSV-formatted rows.
            // Wrap the sheet stream in a StreamReader.
            StreamReader sr = new StreamReader(kvp.Value, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: 4096, leaveOpen: true);
            // Create a CSV virtual data table from the StreamReader.
            // This is our reusable CSV provider.
            CsvVirtualDataTable virtualTable = new CsvVirtualDataTable(sr, kvp.Key, pageSize, guessRows, preferredFloatingPointDataType, guessTypeFunction);
            // If the sheet was empty, you might want to fall back to a previously saved schema.
            // (For now we assume that each sheet contains at least a header row.)
            Tables.Add(virtualTable);
        }
    }

    /// <summary>
    /// Frees the underlying streams by disposing the XLS splitter.
    /// </summary>
    public void FreeStreams()
    {
        splitter.Dispose();
    }

    /// <summary>
    /// Disposes this XlsDatabaseVirtualDataSet and frees all underlying resources.
    /// </summary>
    public new void Dispose()
    {
        if (!disposed)
        {
            FreeStreams();
            base.Dispose();
            disposed = true;
        }
    }
}