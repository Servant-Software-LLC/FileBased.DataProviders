namespace Data.Common.DataSource;

/// <summary>
/// Defines a contract for providing a data source. Implementations of this interface 
/// can use different storage mechanisms, such as the file system or in-memory streams.
/// </summary>
public interface IDataSourceProvider
{
    /// <summary>
    /// Gets the type of the data source (e.g., Directory or File).
    /// </summary>
    DataSourceType DataSourceType { get; }

    /// <summary>
    /// Checks if the storage exists for the specified table.
    /// </summary>
    /// <param name="tableName">The name of the table to check.</param>
    /// <returns>True if the storage exists; otherwise, false.</returns>
    bool StorageExists(string tableName);

    /// <summary>
    /// Lists all tables of the data source
    /// </summary>
    /// <returns></returns>
    IEnumerable<string> GetTableNames();

    /// <summary>
    /// Gets a <see cref="TextReader"/> to read data from the specified table.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <returns>A <see cref="TextReader"/> for reading the table data.</returns>
    TextReader GetTextReader(string tableName);

    /// <summary>
    /// Gets a <see cref="TextWriter"/> to write data to the specified table.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <returns>A <see cref="TextWriter"/> for writing the table data.</returns>
    TextWriter GetTextWriter(string tableName);

    /// <summary>
    /// Gets a unique storage identifier for the specified table.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <returns>The storage identifier for the table.</returns>
    string StorageIdentifier(string tableName);

    /// <summary>
    /// Starts watching the data source for changes.
    /// </summary>
    void StartWatching();

    /// <summary>
    /// Stops watching the data source for changes.
    /// </summary>
    void StopWatching();

    /// <summary>
    /// Ensures that the watcher that looks for data source changes is properly initialized.
    /// </summary>
    void EnsureWatcher();

    /// <summary>
    /// Occurs when a table in the data source changes externally to the ADO.NET Data Provider.
    /// </summary>
    public event DataSourceEventHandler Changed;
}
