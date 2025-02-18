using Microsoft.Data.Analysis;
using Data.Common.Utils.ConnectionString;
using SqlBuildingBlocks.POCOs;
using Data.Common.DataSource;

namespace Data.Csv.Utils;

/// <summary>
/// Represents a virtualized CSV data table that pages in rows on demand from a large CSV file.
/// This class inherits from <see cref="VirtualDataTable"/> and sets its <see cref="VirtualDataTable.Columns"/>
/// and <see cref="VirtualDataTable.Rows"/> properties by reading the CSV file in pages using a DataFrame.
/// This approach allows processing CSV files that are too large to load entirely into memory.
/// </summary>
public class CsvVirtualDataTable : VirtualDataTable, IDisposable
{
    private readonly IDataSourceProvider _dataSourceProvider;
    private readonly int _pageSize;
    private readonly int _guessRows;
    private readonly FloatingPointDataType _preferredFloatingPointDataType;
    private Type[] _columnTypes = Array.Empty<Type>();

    // Hold on to the underlying stream so we can keep paging through it.
    private readonly CsvTransformStream _transformStream;
    private readonly StreamReader _reader;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="CsvVirtualDataTable"/> class.
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
    public CsvVirtualDataTable(
        IDataSourceProvider dataSourceProvider,
        string tableName,
        int pageSize,
        int guessRows,
        FloatingPointDataType preferredFloatingPointDataType)
        : base(tableName)
    {
        _dataSourceProvider = dataSourceProvider ?? throw new ArgumentNullException(nameof(dataSourceProvider));
        _pageSize = pageSize;
        _guessRows = guessRows;
        _preferredFloatingPointDataType = preferredFloatingPointDataType;

        // Instead of using "using", store the reader and transform stream for later use.
        _reader = _dataSourceProvider.GetTextReader(TableName);
        _transformStream = new CsvTransformStream(_reader);

        // Determine the schema and column data types using the first page.
        var firstPageDataRows = InitializeSchemaAndColumnTypes();


        // Set the Rows property to a lazy iterator that pages in DataRows on demand.
        Rows = GetRowsIterator(firstPageDataRows);
    }

    /// <summary>
    /// Initializes the schema (the Columns property) and determines column data types from the first page of the CSV.
    /// </summary>
    private IEnumerable<DataRow> InitializeSchemaAndColumnTypes()
    {
        // Load the first page of data (which also reads the header).
        DataFrame firstPage = DataFrame.LoadCsv(_transformStream, numberOfRowsToRead: _pageSize, guessRows: _guessRows);

        DataTable schemaTable = new DataTable(TableName);
        if (firstPage.Columns.Count > 0)
        {
            // Create columns using the names and data types determined from the first page of data.
            foreach (var col in firstPage.Columns)
            {
                DataColumn dc = new DataColumn(col.Name, col.DataType);
                schemaTable.Columns.Add(dc);
            }
            // Store the determined column types for subsequent pages.
            _columnTypes = firstPage.Columns.Select(column => _preferredFloatingPointDataType.GetClrType(column.DataType)).ToArray();
        }
        else if (!string.IsNullOrEmpty(_transformStream.HeaderLine))
        {
            // Fallback: if DataFrame didn't yield columns, use the header line with string type.
            var columnNames = _transformStream.HeaderLine.Split(',');
            foreach (var name in columnNames)
            {
                schemaTable.Columns.Add(new DataColumn(name.Trim(), typeof(string)));
            }
            _columnTypes = schemaTable.Columns.Cast<DataColumn>().Select(c => c.DataType).ToArray();
        }

        // Set the Columns property (inherited from VirtualDataTable) with the determined schema.
        Columns = schemaTable.Columns;

        return ToDataRows(firstPage);
    }

    /// <summary>
    /// Returns an iterator that lazily loads rows from the CSV file in pages using the predetermined column types.
    /// </summary>
    /// <returns>An enumerable sequence of <see cref="DataRow"/>.</returns>
    private IEnumerable<DataRow> GetRowsIterator(IEnumerable<DataRow> firstPageDataRows)
    {
        //First enumerate on the rows of the first page that was loaded when determining data types.
        int numberOfFirstPageRows = 0;
        foreach(DataRow dataRow in firstPageDataRows)
        {
            numberOfFirstPageRows++;
            yield return dataRow;
        }

        // If fewer rows than the page size were read, assume we've reached the end of the CSV.
        if (numberOfFirstPageRows < _pageSize)
            yield break;

        while (true)
        {
            // Load a page using the predetermined column types so that no further type guessing occurs.
            DataFrame page = DataFrame.LoadCsv(_transformStream, numberOfRowsToRead: _pageSize, guessRows: _guessRows, dataTypes: _columnTypes);
            if (page.Rows.Count == 0)
                yield break;

            var pageDataRows = ToDataRows(page);
            foreach (DataRow row in pageDataRows)
            {
                yield return row;
            }

            // If fewer rows than the page size were read, assume we've reached the end of the CSV.
            if (page.Rows.Count < _pageSize)
                yield break;
        }
    }

    private IEnumerable<DataRow> ToDataRows(DataFrame dataFrame)
    {
        // Create a schema DataTable to construct new DataRow objects.
        DataTable schemaTable = CreateEmptyDataTable();

        for (int rowIndex = 0; rowIndex < dataFrame.Rows.Count; rowIndex++)
        {
            DataRow newRow = schemaTable.NewRow();
            for (int colIndex = 0; colIndex < schemaTable.Columns.Count; colIndex++)
            {
                newRow[colIndex] = dataFrame.Columns[colIndex][rowIndex];
            }
            yield return newRow;
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _transformStream?.Dispose();
            _reader?.Dispose();
            _disposed = true;
        }
    }
}
