using Data.Common.Utils.ConnectionString;
using SqlBuildingBlocks.POCOs;

namespace Data.Xls.Utils;

/// <summary>
/// Represents virtualized XLS data tables that page in rows on demand from a large XLS file.
/// This class inherits from <see cref="VirtualDataTable"/> and sets its <see cref="VirtualDataTable.Columns"/>
/// and <see cref="VirtualDataTable.Rows"/> properties by reading the XLS file in pages using a DataFrame.
/// This approach allows processing XLS files that are too large to load entirely into memory.
/// </summary>
public class XlsVirtualDataTable : VirtualDataTable, IDisposable
{
    private readonly int _guessRows;
    private readonly int _pageSize;
    private readonly FloatingPointDataType _preferredFloatingPointDataType;
    private readonly Func<IEnumerable<string>, Type> _guessTypeFunction;

    // Hold on to the underlying stream so we can keep paging through it.
    private readonly StreamReader _baseReader;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="XlsVirtualDataTable"/> class.
    /// </summary>
    /// <param name="dataSourceProvider">
    /// The data source provider used to obtain a text reader for the CSV file.
    /// </param>
    /// <param name="tableName">
    /// The name of the table (and CSV file) to load.
    /// </param>
    /// <param name="pageSize">
    /// The number of rows to load per page.
    /// </param>
    /// <param name="guessRows">
    /// The number of rows to use when guessing data types.
    /// </param>
    /// <param name="preferredFloatingPointDataType">
    /// The preferred floating point data type for numeric columns.
    /// </param>
    public XlsVirtualDataTable(
        StreamReader streamReader,
        string tableName,
        int pageSize,
        int guessRows,
        FloatingPointDataType preferredFloatingPointDataType,
        Func<IEnumerable<string>, Type> guessTypeFunction)
        : base(tableName)
    {
        _pageSize = pageSize;
        _guessRows = guessRows > 0 ? guessRows : throw new ArgumentOutOfRangeException(nameof(guessRows), $"Guess row must be greater than 0.  GuessRows: {guessRows}");
        _preferredFloatingPointDataType = preferredFloatingPointDataType;
        _guessTypeFunction = guessTypeFunction;

        // Instead of using "using", store the reader and transform stream for later use.
        _baseReader = streamReader ?? throw new ArgumentNullException(nameof(streamReader));

        // Determine the schema and column data types using the first page.
        DetermineColumns();

        // Set the Rows property to a lazy iterator that pages in DataRows on demand.
        Rows = GetRowsIterator();
    }

    private void DetermineColumns()
    {
        Columns.Clear();

        //Foreach column discovered
        //Columns.Add(new DataColumn(name, dataType));

    }

    /// <summary>
    /// Returns an iterator that lazily loads rows from the CSV file in pages using the predetermined column types.
    /// </summary>
    /// <returns>An enumerable sequence of <see cref="DataRow"/>.</returns>
    private IEnumerable<DataRow> GetRowsIterator()
    {
        yield break;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _baseReader.Dispose();
            _disposed = true;
        }
    }
}
