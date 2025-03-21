using Data.Common.Utils;
using System.Text;

namespace Data.Common.DataSource;

/// <summary>
/// Provides a data source implementation that stores table data in memory using streams. 
/// This is useful for scenarios where data is transient or does not need to be persisted to the file system.
/// </summary>
public abstract class StreamedDataSource : IDataSourceProvider, IDisposable
{
    protected readonly StreamedTableManager tables = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamedDataSource"/> class.
    /// </summary>
    protected StreamedDataSource(string database) =>
        Database = !string.IsNullOrEmpty(database) ? database : throw new ArgumentNullException(nameof(database));

    /// <summary>
    /// Name of the database
    /// </summary>
    public string Database { get; private set; }

    /// <summary>
    /// Gets the type of the data source, which is <see cref="DataSourceType.Directory"/> for this implementation.
    /// </summary>
    public virtual DataSourceType DataSourceType => DataSourceType.Directory;

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

    protected virtual void OnChanged(string tableName) => Changed?.Invoke(this, new DataSourceEventArgs(tableName));

    public void Dispose()
    {
        tables.Dispose();
    }
}
