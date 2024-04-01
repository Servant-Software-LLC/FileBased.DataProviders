using Data.Common.Utils;
using Microsoft.Extensions.Logging;

namespace System.Data.FileClient;


/// <summary>
/// Represents a connection to a file-based database.
/// </summary>
/// <typeparam name="TFileParameter">The type of file parameter.</typeparam>
public abstract class FileConnection<TFileParameter> : DbConnection, IFileConnection, IFileConnectionInternal, IConnectionStringProperties
    where TFileParameter : FileParameter<TFileParameter>, new()
{
    private readonly FileConnectionString connectionString = new();
    private ConnectionState state;

    /// <summary>
    /// Gets the file extension of the database file.
    /// </summary>
    public abstract string FileExtension { get; }

    /// <summary>
    /// Gets or sets the connection string used to open the database.
    /// </summary>
    public override string ConnectionString { get => connectionString.ConnectionString!; set => connectionString.ConnectionString = value; }

    /// <summary>
    /// Gets the data source of the database.
    /// </summary>
    public override string DataSource => connectionString.DataSource;

    /// <summary>
    /// Gets a value indicating whether the database is formatted.
    /// </summary>
    public bool? Formatted => connectionString.Formatted;

    /// <summary>
    /// Gets the log level of the database.
    /// </summary>
    public LogLevel LogLevel => connectionString.LogLevel ?? LogLevel.None;

    /// <summary>
    /// Gets a value indicating whether to create the database if it does not exist.
    /// </summary>
    public bool CreateIfNotExist => connectionString.CreateIfNotExist ?? false;

    /// <summary>
    /// Gets the name of the database.
    /// </summary>
    public override string Database => connectionString.DataSource ?? string.Empty;

    /// <summary>
    /// Gets the state of the connection.
    /// </summary>
    public override ConnectionState State => state;

    /// <summary>
    /// Provides consumers potential extensibility to bring their own custom DataSetWriters.  For example: JSON DataSetWriter that injects JSON comments into stored JSON files.
    /// </summary>
    protected abstract Func<FileStatement, IDataSetWriter> CreateDataSetWriter { get; }
    Func<FileStatement, IDataSetWriter> IFileConnectionInternal.CreateDataSetWriter => CreateDataSetWriter;

    internal LoggerServices LoggerServices { get; }
    LoggerServices IFileConnection.LoggerServices => LoggerServices;
    private ILogger<FileConnection<TFileParameter>> log => LoggerServices.CreateLogger<FileConnection<TFileParameter>>();

    /// <summary>
    /// Initializes a new instance of the <see cref="FileConnection{TFileParameter}"/> class.
    /// </summary>
    /// <param name="connectionString">The connection string used to open the database.</param>
    public FileConnection(FileConnectionString connectionString) 
    {
        if (connectionString is null)
            throw new ArgumentNullException(nameof(connectionString));

        this.connectionString = connectionString;
        state = ConnectionState.Closed;

        LoggerServices = new LoggerServices(LogLevel);
        log.LogInformation($"{GetType()} created.  ConnectionString = {connectionString}");
    }

    /// <summary>
    /// Gets the server version of the database.
    /// </summary>
    public override string ServerVersion
    {
        get
        {
            var assembly = Reflection.Assembly.GetExecutingAssembly();
            var fvi = Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;

            log.LogDebug($"{GetType()}.{nameof(ServerVersion)}() = {version}");
            return version;
        }
    }

    /// <summary>
    /// Gets the path type of the database.
    /// </summary>
    public PathType PathType => this.GetPathType();

    /// <summary>
    /// Gets a value indicating whether the database is a folder.
    /// </summary>
    public bool FolderAsDatabase => PathType == PathType.Directory;

    /// <summary>
    /// Gets a value indicating whether the database is in admin mode.
    /// </summary>
    public bool AdminMode => PathType == PathType.Admin;

    /// <summary>
    /// Gets the file reader for the database.
    /// </summary>
    public FileReader FileReader { get; protected set; }

    /// <summary>
    /// Gets a value indicating whether the database is case-insensitive.
    /// </summary>
    public bool CaseInsensitive => true;

    /// <summary>
    /// Begins a new transaction.
    /// </summary>
    /// <returns>The new transaction.</returns>
    public new abstract FileTransaction<TFileParameter> BeginTransaction();

    /// <summary>
    /// Begins a new transaction with the specified isolation level.
    /// </summary>
    /// <param name="il">The isolation level.</param>
    /// <returns>The new transaction.</returns>
    public new abstract FileTransaction<TFileParameter> BeginTransaction(IsolationLevel il);

    IDbTransaction IDbConnection.BeginTransaction() => BeginTransaction();

    IDbTransaction IDbConnection.BeginTransaction(IsolationLevel il) => BeginTransaction(il);

    /// <summary>
    /// Begins a new transaction with the specified isolation level.
    /// </summary>
    /// <param name="isolationLevel">The isolation level.</param>
    /// <returns>The new transaction.</returns>
    protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) => BeginTransaction(isolationLevel);

    public abstract FileDataAdapter<TFileParameter> CreateDataAdapter(string query);
    
    /// <summary>
    /// Changes the database to the specified database name.
    /// </summary>
    /// <param name="databaseName">The name of the database.</param>
    public override void ChangeDatabase(string databaseName)
    {
        if (string.IsNullOrEmpty(databaseName))
            throw new ArgumentNullException(nameof(databaseName));

        connectionString.DataSource = databaseName;

        //Other ADO.NET providers that support automatically creating a database when provided in
        //the connection string, either still require that for this operation that the database
        //already exists (SQL Server LocalDB) or just throws a NotSupportedException regardless
        //of whether the database exists or not (SQLite).
        //Therefore, we will not automatically create the database if it doesn't exist and throw.
        ThrowHelper.ThrowIfInvalidPath(PathType, databaseName);
    }

    /// <summary>
    /// Closes the connection.
    /// </summary>
    public override void Close() => state = ConnectionState.Closed;

    /// <summary>
    /// Creates a new command.
    /// </summary>
    /// <returns>The new command.</returns>
    public new abstract FileCommand<TFileParameter> CreateCommand();

    /// <summary>
    /// Creates a new command with the specified command text.
    /// </summary>
    /// <param name="cmdText">The command text.</param>
    /// <returns>The new command.</returns>
    public abstract FileCommand<TFileParameter> CreateCommand(string cmdText);

    IDbCommand IDbConnection.CreateCommand() => CreateCommand();

    protected override DbCommand CreateDbCommand() => CreateCommand();

    /// <summary>
    /// Creates the file as a database.
    /// </summary>
    /// <param name="databaseFileName">The name of the database file.</param>
    protected internal abstract void CreateFileAsDatabase(string databaseFileName);
    
    /// <summary>
    /// Opens the connection.
    /// </summary>
    public override void Open()
    {
        if (!CreateIfNotExist)
            ThrowHelper.ThrowIfInvalidPath(PathType, Database);
        else
        {
            if (PathType == PathType.None)
                FileCreateDatabase<TFileParameter>.Execute(this, Database);
        }

        state = ConnectionState.Open;
    }

    /// <summary>
    /// Disposes the connection.
    /// </summary>
    protected new void Dispose()
    {
        base.Dispose();
        state = ConnectionState.Closed;
    }


    internal static PathType GetPathType(string database, string providerFileExtension)
    {
        if (FileConnectionExtensions.IsAdmin(database))
            return PathType.Admin;

        //Does this look like a file with the appropriate file extension?
        var fileExtension = Path.GetExtension(database);
        if (!string.IsNullOrEmpty(fileExtension) && string.Compare(fileExtension, $".{providerFileExtension}", true) == 0)
            return PathType.File;

        //Assume that this is referring to FolderAsDatabase
        return PathType.Directory;
    }

}
