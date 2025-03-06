using Data.Common.Utils;
using System.Text;

namespace Data.Common.DataSource;

/// <summary>
/// Provides a data source implementation that stores table data in memory using streams. 
/// This is useful for scenarios where data is transient or does not need to be persisted to the file system.
/// </summary>
public class StreamedDataSource : IDataSourceProvider, IDisposable
{
    private readonly StreamedTableManager tables = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamedDataSource"/> class.
    /// </summary>
    public StreamedDataSource() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamedDataSource"/> class.
    /// </summary>
    /// <param name="tableName">The name of the initial table.</param>
    /// <param name="utf8TableData">The stream containing UTF-8 encoded data for the initial table.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when either <paramref name="tableName"/> or <paramref name="utf8TableData"/> is null.
    /// </exception>
    public StreamedDataSource(string tableName, Stream stream) => AddTable(tableName, () => stream);

    public StreamedDataSource(string tableName, Func<Stream> streamCreationFunc) => AddTable(tableName, streamCreationFunc);

    /// <summary>
    /// Adds a new table and its associated data stream to the data source.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <param name="utf8TableData">The stream containing UTF-8 encoded data for the table.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when either <paramref name="tableName"/> or <paramref name="utf8TableData"/> is null.
    /// </exception>
    public void AddTable(string tableName, Func<Stream> streamCreationFunc)
    {
        tables.AddTable(tableName, streamCreationFunc);
        Changed?.Invoke(this, new DataSourceEventArgs(tableName));
    }

    public void AddTable(string tableName, Stream stream) => AddTable(tableName, ()=>stream);

    /// <summary>
    /// Gets the type of the data source, which is <see cref="DataSourceType.Directory"/> for this implementation.
    /// </summary>
    public DataSourceType DataSourceType => DataSourceType.Directory;

    /// <summary>
    /// Checks if storage exists for the specified table.
    /// </summary>
    /// <param name="tableName">The name of the table to check.</param>
    /// <returns>True if the table exists in the data source; otherwise, false.</returns>
    public bool StorageExists(string tableName) => tables.StorageExists(tableName);

    public IEnumerable<string> GetTableNames() => tables.GetTableNames();

    /// <summary>
    /// Gets a <see cref="TextReader"/> for the specified table.
    /// </summary>
    /// <param name="tableName">The name of the table to read.</param>
    /// <returns>A <see cref="TextReader"/> for reading the table data.</returns>
    /// <exception cref="FileNotFoundException">Thrown if the specified table does not exist in the data source.</exception>
    public StreamReader GetTextReader(string tableName)
    {
        var bufferedStream = tables.GetReadingStream(tableName);

        return new StreamReader(bufferedStream, Encoding.UTF8, true, 1024, true);
    }

    /// <summary>
    /// Gets a <see cref="TextWriter"/> for the specified table.
    /// </summary>
    /// <param name="tableName">The name of the table to write to.</param>
    /// <returns>A <see cref="TextWriter"/> for writing the table data.</returns>
    public TextWriter GetTextWriter(string tableName)
    {
        var memoryStream = tables.GetWritingStream(tableName);
        return new StreamWriter(memoryStream, Encoding.UTF8, 1024, true);
    }

    /// <summary>
    /// Gets the storage identifier for the specified table.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <returns>The storage identifier, which is the table name for this data source.</returns>
    public string StorageIdentifier(string tableName) => tableName;

    /// <summary>
    /// Starts watching the data source for changes. (Not applicable for in-memory streams.)
    /// </summary>
    public void StartWatching() { }

    /// <summary>
    /// Stops watching the data source for changes. (Not applicable for in-memory streams.)
    /// </summary>
    public void StopWatching() { }

    /// <summary>
    /// Ensures the file system watcher is initialized. (Not applicable for in-memory streams.)
    /// </summary>
    public void EnsureWatcher() { }

    /// <summary>
    /// Occurs when a table in the data source is changed. (Not applicable for in-memory streams.)
    /// </summary>
    public event DataSourceEventHandler Changed;

    public void Dispose()
    {
        tables.Dispose();
    }
}
