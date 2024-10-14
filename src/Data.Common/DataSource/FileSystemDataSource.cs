namespace Data.Common.DataSource;

/// <summary>
/// Provides a data source implementation for file system-based storage.
/// </summary>
public class FileSystemDataSource : IDataSourceProvider
{
    private readonly string path;
    private readonly string fileExtension;
    private FileSystemWatcher fileWatcher;
    private readonly IFileConnection fileConnection;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileSystemDataSource"/> class.
    /// </summary>
    /// <param name="path">The path to the directory or file used as the data source.</param>
    /// <param name="dataSourceType">The type of the data source (Directory or File).</param>
    /// <param name="fileExtension">The file extension used for table files when the data source type is Directory.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided path is null or empty.</exception>
    public FileSystemDataSource(string path, DataSourceType dataSourceType, string fileExtension, IFileConnection fileConnection)
    {
        this.path = string.IsNullOrEmpty(path) ? throw new ArgumentNullException(nameof(path)) : path;
        DataSourceType = dataSourceType;
        this.fileExtension = fileExtension;
        this.fileConnection = fileConnection ?? throw new ArgumentNullException(nameof(fileConnection));
    }

    /// <summary>
    /// Gets the type of the data source.
    /// </summary>
    public DataSourceType DataSourceType { get; }

    /// <summary>
    /// Checks if the file exists for the specified table name.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <returns>True if the file exists; otherwise, false.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the data source type is not supported.</exception>
    public bool StorageExists(string tableName)
    {
        switch (DataSourceType)
        {
            case DataSourceType.Directory:
                return File.Exists(GetTablePath(tableName));
            case DataSourceType.File:
                return File.Exists(path);
            default:
                throw new InvalidOperationException($"Cannot check storage for a data source of type {DataSourceType}");
        }
    }

    /// <summary>
    /// Gets a <see cref="TextReader"/> for the specified table name.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <returns>A <see cref="TextReader"/> for reading the table data.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the data source type is not supported.</exception>
    public TextReader GetTextReader(string tableName) => DataSourceType switch
    {
        DataSourceType.Directory => GetTextReader_FolderAsDB(tableName),
        DataSourceType.File => GetTextReader_FileAsDB(),
        _ => throw new InvalidOperationException($"Cannot get a TextReader for a data source of type {DataSourceType}")
    };

    /// <summary>
    /// Gets a <see cref="TextWriter"/> for the specified table name.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <returns>A <see cref="TextWriter"/> for writing the table data.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the data source type is not supported.</exception>
    public TextWriter GetTextWriter(string tableName) => DataSourceType switch
    {
        DataSourceType.Directory => GetTextWriter_FolderAsDB(tableName),
        DataSourceType.File => GetTextWriter_FileAsDB(),
        _ => throw new InvalidOperationException($"Cannot get a TextWriter for a data source of type {DataSourceType}")
    };

    /// <summary>
    /// Gets the name of the file name for the specified table name or database (in the case of FileAsDB).
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <returns>The storage identifier for the table.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the data source type is not supported.</exception>
    public string StorageIdentifier(string tableName) => DataSourceType switch
    {
        DataSourceType.Directory => GetTableFileName(tableName),
        DataSourceType.File => Path.GetFileName(path),
        _ => throw new InvalidOperationException($"Cannot get a storage identifier for a data source of type {DataSourceType}")
    };

    /// <summary>
    /// Starts watching the file system for changes in the data source.
    /// </summary>
    public void StartWatching()
    {
        if (fileWatcher != null)
        {
            fileWatcher.Changed -= FileWatcher_Changed;
            fileWatcher.Changed += FileWatcher_Changed;
        }
    }

    /// <summary>
    /// Stops watching the file system for changes in the data source.
    /// </summary>
    public void StopWatching()
    {
        if (fileWatcher != null)
            fileWatcher.Changed -= FileWatcher_Changed;
    }

    /// <summary>
    /// Ensures that the file system watcher is properly initialized and active.
    /// </summary>
    public void EnsureWatcher()
    {
        if (fileWatcher == null)
        {
            fileWatcher = new FileSystemWatcher();
            fileWatcher.NotifyFilter = NotifyFilters.LastWrite;
            if (fileConnection.FolderAsDatabase)
            {
                fileWatcher.Path = fileConnection.Database;
                fileWatcher.Filter = $"*.{fileConnection.FileExtension}";
            }
            else
            {
                var file = new FileInfo(fileConnection.Database);
                fileWatcher.Path = file.DirectoryName!;
                fileWatcher.Filter = file.Name;
            }
            fileWatcher.EnableRaisingEvents = true;
        }
    }

    /// <summary>
    /// Occurs when a file in the data source is changed externally.
    /// </summary>
    public event DataSourceEventHandler Changed;

    private TextReader GetTextReader_FolderAsDB(string tableName)
    {
        var pathToTableFile = GetTablePath(tableName);
        return GetTextReaderFromFilePath(pathToTableFile);
    }

    private void FileWatcher_Changed(object sender, FileSystemEventArgs e) =>
        Changed?.Invoke(sender, new DataSourceEventArgs(Path.GetFileNameWithoutExtension(e.FullPath)));

    private TextReader GetTextReader_FileAsDB() => GetTextReaderFromFilePath(path);

    private TextReader GetTextReaderFromFilePath(string path)
    {
        // Open the file stream
        FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);

        // Create and return a StreamReader (which is a TextReader)
        return new StreamReader(fileStream);
    }

    private TextWriter GetTextWriter_FolderAsDB(string tableName)
    {
        var pathToTableFile = GetTablePath(tableName);
        return GetTextWriterFromFilePath(pathToTableFile);
    }

    private TextWriter GetTextWriter_FileAsDB() => GetTextWriterFromFilePath(path);
    
    private TextWriter GetTextWriterFromFilePath(string path)
    {
        // Open the file stream
        FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);

        // Create and return a StreamWriter (which is a TextWriter)
        return new StreamWriter(fileStream);
    }

    /// <summary>
    /// Provides a full file path to the table file.
    /// </summary>
    /// <param name="fileConnection"></param>
    /// <param name="tableName">Name of table</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private string GetTablePath(string tableName)
    {
        if (DataSourceType != DataSourceType.Directory)
            throw new InvalidOperationException($"Can only call the {nameof(GetTablePath)} method on a data source of type {nameof(DataSourceType.Directory)}. DataSourceType={DataSourceType}");

        return Path.Combine(path, GetTableFileName(tableName));
    }

    private string GetTableFileName(string tableName) => $"{tableName}.{fileExtension}";

}
