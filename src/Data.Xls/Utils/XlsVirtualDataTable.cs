using System.Text.RegularExpressions;
using Data.Common.Utils.ConnectionString;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
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
    private Stream _stream;
    private readonly int _guessRows;
    private readonly int _pageSize;
    private readonly Func<IEnumerable<string>, Type> _guessTypeFunction;
    private Dictionary<string, string> _columnLetterHeadingMap;
    private readonly FloatingPointDataType _preferredFloatingPointDataType;
    
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
        Stream stream,
        string tableName,
        int guessRows,
        FloatingPointDataType preferredFloatingPointDataType,
        Func<IEnumerable<string>, Type> guessTypeFunction)
        : base(tableName)
    {
        _stream = stream ?? throw new ArgumentNullException(nameof(stream));
        _guessRows = guessRows > 0 ? guessRows : throw new ArgumentOutOfRangeException(nameof(guessRows), $"Guess row must be greater than 0.  GuessRows: {guessRows}");
        _guessTypeFunction = guessTypeFunction;
        _columnLetterHeadingMap = new Dictionary<string, string>();
        _preferredFloatingPointDataType = preferredFloatingPointDataType;

        // Determine the schema and column data types using the first page.
        DetermineColumns();

        // Set the Rows property to a lazy iterator that pages in DataRows on demand.
        Rows = XslxReader.GetRowsIterator(_stream, this);
    }

    private void DetermineColumns()
    {
        var columnsOfData = XslxReader.GetColumnValues(_stream, _guessRows);
        var columnTypes = GuessTypes(columnsOfData);
        
        foreach (var columnType in columnTypes)
        {
            AddColumn(columnType.Key, columnType.Value);
        }
        
        if (_stream.CanSeek)
        {
            _stream.Seek(0, SeekOrigin.Begin);
        }
    }

    private void AddColumn(string columnName, Type columnType)
    {
        var existingColumnIndex = Columns.IndexOf(columnName);
        if (existingColumnIndex == -1)
        {
            //This is a new column, which is the common scenario
            Columns.Add(columnName, columnType);
            return;
        }

        var existingColumn = Columns[existingColumnIndex];
        if (existingColumn.DataType != columnType) 
        {
            throw new InvalidOperationException($"The column {columnName} already exists in this virtual table. Further, the data types don't match.  Existing: {existingColumn.DataType.FullName} New: {columnType.FullName}");
        }
    }
    
    private Dictionary<string, Type> GuessTypes(IDictionary<string, List<string>> columnsOfData)
    {
        var columnTypesMap = new Dictionary<string, Type>();
        foreach (var pair in columnsOfData)
        {
            var type = TypeGuesser.GuessType(pair.Value);
            columnTypesMap.Add(pair.Key, _preferredFloatingPointDataType.GetClrType(type));
        }
        
        return columnTypesMap;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _stream.Dispose();
            _disposed = true;
        }
    }
}
