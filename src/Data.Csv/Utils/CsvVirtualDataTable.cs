using Microsoft.Data.Analysis;
using Data.Common.Utils.ConnectionString;
using SqlBuildingBlocks.POCOs;
using System.Text;

namespace Data.Csv.Utils;

/// <summary>
/// Represents a virtualized CSV data table that pages in rows on demand from a large CSV file.
/// This class inherits from <see cref="VirtualDataTable"/> and sets its <see cref="VirtualDataTable.Columns"/>
/// and <see cref="VirtualDataTable.Rows"/> properties by reading the CSV file in pages using a DataFrame.
/// This approach allows processing CSV files that are too large to load entirely into memory.
/// </summary>
public class CsvVirtualDataTable : VirtualDataTable, IDisposable
{
    private readonly int _pageSize;
    private readonly int _guessTypeRows;
    private readonly FloatingPointDataType _preferredFloatingPointDataType;
    private readonly Func<IEnumerable<string>, Type> _guessTypeFunction;

    // Hold on to the underlying stream so we can keep paging through it.
    private readonly CsvTransformStream _transformStream;
    private readonly StreamReader _baseReader;
    private readonly StreamReader _transformReader;
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
    /// <param name="guessTypeRows">
    /// The number of rows to use when guessing data types.
    /// </param>
    /// <param name="preferredFloatingPointDataType">
    /// The preferred floating point data type for numeric columns.
    /// </param>
    public CsvVirtualDataTable(
            StreamReader streamReader,
            string tableName,
            int pageSize,
            int guessTypeRows,
            FloatingPointDataType preferredFloatingPointDataType,
            Func<IEnumerable<string>, Type> guessTypeFunction,
            char separator
        )
        : base(tableName)
    {
        _pageSize = pageSize;
        _guessTypeRows = guessTypeRows > 0 ? guessTypeRows : throw new ArgumentOutOfRangeException(nameof(guessTypeRows), $"Guess type row must be greater than 0.  GuessRows: {guessTypeRows}");
        _preferredFloatingPointDataType = preferredFloatingPointDataType;
        _guessTypeFunction = guessTypeFunction;

        // Instead of using "using", store the reader and transform stream for later use.
        _baseReader = streamReader ?? throw new ArgumentNullException(nameof(streamReader));
        _transformStream = new CsvTransformStream(_baseReader, separator);
        _transformReader = new StreamReader(_transformStream, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: 4096, leaveOpen: true);

        // Determine the schema and column data types using the first page.
        DetermineColumns(separator);

        // Set the Rows property to a lazy iterator that pages in DataRows on demand.
        Rows = GetRowsIterator(separator);
    }

    private void DetermineColumns(char separator)
    {
        // Load the first page of data (which also reads the header).  Note:  Using the _transformStream on this LoadCsv, because we do want a Seek to
        // origin on the stream.  Since we're not using the data, limit numberOfRowsToRead to _guessRows.
        DataFrame firstPage = DataFrame.LoadCsv(_transformStream, numberOfRowsToRead: _guessTypeRows, guessRows: _guessTypeRows,
                                                guessTypeFunction: _guessTypeFunction, separator: separator);

        Columns.Clear();
        if (firstPage.Columns.Count > 0)
        {
            // Create columns using the names and data types determined from the first page of data.
            foreach (var col in firstPage.Columns)
            {
                Type dataType = _preferredFloatingPointDataType.GetClrType(col.DataType);
                DataColumn dc = new DataColumn(col.Name, dataType);
                Columns.Add(dc);
            }
        }
        else if (!string.IsNullOrEmpty(_transformStream.HeaderLine))
        {
            // Fallback: if DataFrame didn't yield columns, use the header line with string type.
            var columnNamesFromStream = _transformStream.HeaderLine.Split(separator);
            foreach (var name in columnNamesFromStream)
            {
                Columns.Add(new DataColumn(name.Trim(), typeof(string)));
            }
        }
    }

    /// <summary>
    /// Returns an iterator that lazily loads rows from the CSV file in pages using the predetermined column types.
    /// </summary>
    /// <returns>An enumerable sequence of <see cref="DataRow"/>.</returns>
    private IEnumerable<DataRow> GetRowsIterator(char separator)
    {
        bool firstPage = true;
        _transformStream.Seek(0, SeekOrigin.Begin);

        while (!_transformReader.EndOfStream)
        {
            // Load a page using the predetermined column types so that no further type guessing occurs.
            var columns = Columns.Cast<DataColumn>().ToList();
            string[] columnNames = columns.Select(col => col.ColumnName).ToArray();
            Type[] columnTypes = columns.Select(col => col.DataType).ToArray();

            var pageData = ReadPageData(firstPage);
            if (string.IsNullOrWhiteSpace(pageData)) 
            {
                yield break;
            }

            DataFrame page = DataFrame.LoadCsvFromString(pageData, header: firstPage, columnNames: columnNames, dataTypes: columnTypes, separator: separator);
            firstPage = false;
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

    private string ReadPageData(bool firstPage)
    {
        StringBuilder stringBuilder = new StringBuilder();

        //Don't count the header of the CSV data
        var pageSize = _pageSize + (firstPage ? 1 : 0);
        for (int i = 0; i < pageSize; i++)
        {
            string line = _transformReader.ReadLine();
            if (line == null)
            {
                break;
            }

            stringBuilder.AppendLine(line);
        }

        return stringBuilder.ToString();
    }

    private IEnumerable<DataRow> ToDataRows(DataFrame dataFrame)
    {
        for (int rowIndex = 0; rowIndex < dataFrame.Rows.Count; rowIndex++)
        {
            DataRow newRow = NewRow();
            for (int colIndex = 0; colIndex < Columns.Count; colIndex++)
            {
                var value = dataFrame.Columns[colIndex][rowIndex] ?? DBNull.Value;
                newRow[colIndex] = value;
            }
            yield return newRow;
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _transformReader.Dispose();
            _transformStream.Dispose();
            _baseReader.Dispose();
            _disposed = true;
        }
    }
}
