using Data.Common.DataSource;
using Data.Common.Utils;
using Microsoft.Extensions.Logging;

namespace System.Data.FileClient;

/// <summary>
/// Represents a connection to a file-based database.
/// </summary>
/// <typeparam name="TFileParameter">The type of file parameter.</typeparam>
public abstract class FileConnection<TFileParameter> : DbConnection, IFileConnection, IFileConnectionInternal
    where TFileParameter : FileParameter<TFileParameter>, new()
{
    private readonly FileConnectionString connectionString = new();
    private ConnectionState state = ConnectionState.Closed;

    /// <summary>
    /// Gets the file extension of the database file.
    /// </summary>
    public abstract string FileExtension { get; }

    /// <summary>
    /// Gets or sets the connection string used to open the database.
    /// </summary>
    public override string ConnectionString 
    { 
        get => connectionString.ConnectionString!;
        set
        {
            connectionString.ConnectionString = value;
            LoggerServices = new LoggerServices(LogLevel);
            FileReader = !AdminMode ? CreateFileReader : null;
        }
    }

    /// <summary>
    /// Gets the data source of the database.
    /// </summary>
    public override string DataSource => connectionString.DataSource;

    public IDataSourceProvider DataSourceProvider { get; set; }

    /// <summary>
    /// Gets a value indicating whether the database is formatted.
    /// </summary>
    public bool? Formatted => connectionString.Formatted;

    /// <summary>
    /// Gets the log level of the database.
    /// </summary>
    public LogLevel LogLevel => connectionString.LogLevel ?? LogLevel.None;

    /// <summary>
    /// Gets the preferred floating point data type.
    /// </summary>
    public FloatingPointDataType PreferredFloatingPointDataType => connectionString.PreferredFloatingPointDataType ?? FloatingPointDataType.Double;

    /// <summary>
    /// Gets a value indicating whether to create the database if it does not exist.
    /// </summary>
    public bool CreateIfNotExist
    {
        get => connectionString.CreateIfNotExist ?? false;
        internal set => connectionString.CreateIfNotExist = value;
    }

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

    internal LoggerServices LoggerServices { get; private set; }
    LoggerServices IFileConnection.LoggerServices => LoggerServices;
    private ILogger<FileConnection<TFileParameter>> log => LoggerServices.CreateLogger<FileConnection<TFileParameter>>();

    protected FileConnection()
    {
        LoggerServices = new LoggerServices(LogLevel.None);
        FileReader = CreateFileReader;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileConnection{TFileParameter}"/> class.
    /// </summary>
    /// <param name="connectionString">The connection string used to open the database.</param>
    protected FileConnection(FileConnectionString connectionString) 
    {
        if (connectionString is null)
            throw new ArgumentNullException(nameof(connectionString));

        ConnectionString = connectionString;
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
    public DataSourceType DataSourceType => DataSourceProvider != null ? DataSourceProvider.DataSourceType : this.GetDataSourceType();

    /// <summary>
    /// Gets a value indicating whether the database is a folder.
    /// </summary>
    public bool FolderAsDatabase => DataSourceType == DataSourceType.Directory;

    /// <summary>
    /// Gets a value indicating whether the database is in admin mode.
    /// </summary>
    public bool AdminMode => DataSourceType == DataSourceType.Admin;

    /// <summary>
    /// Gets the file reader for the database.
    /// </summary>
    public FileReader FileReader { get; private set; }

    protected abstract FileReader CreateFileReader { get; }

    /// <summary>
    /// Gets a value indicating whether the database is case-insensitive.
    /// </summary>
    public bool CaseInsensitive => true;

    /// <summary>
    /// Indicates whether all data columns are always a string.
    /// </summary>
    public virtual bool DataTypeAlwaysString => false;

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

    public abstract IFileDataAdapter CreateDataAdapter(string query);
    
    /// <summary>
    /// Changes the database to the specified database name.
    /// </summary>
    /// <param name="databaseName">The name of the database.</param>
    public override void ChangeDatabase(string databaseName)
    {
        if (string.IsNullOrEmpty(databaseName))
            throw new ArgumentNullException(nameof(databaseName));

        DataSourceProvider = null;
        connectionString.DataSource = databaseName;

        //Other ADO.NET providers that support automatically creating a database when provided in
        //the connection string, either still require that for this operation that the database
        //already exists (SQL Server LocalDB) or just throws a NotSupportedException regardless
        //of whether the database exists or not (SQLite).
        //Therefore, we will not automatically create the database if it doesn't exist and throw.
        ThrowHelper.ThrowIfInvalidPath(DataSourceType, databaseName);
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
        if (DataSourceProvider is null)
        {
            if (!CreateIfNotExist)
            {
                ThrowHelper.ThrowIfInvalidPath(DataSourceType, Database);
            }
            else
            {
                if (DataSourceType == DataSourceType.None)
                    FileCreateDatabase<TFileParameter>.Execute(this, Database);
            }

            var dataSourceType = DataSourceType;
            if (dataSourceType == DataSourceType.Directory || dataSourceType == DataSourceType.File)
            {
                DataSourceProvider = new FileSystemDataSource(Database, dataSourceType, FileExtension, this);
            }
        }
        else
        {
            // If the data source provider is set, then properly set the connection string to custom.
            if (string.IsNullOrEmpty(ConnectionString))
            {
                ConnectionString = FileConnectionString.CustomDataSource;                
            }
        }

        state = ConnectionState.Open;
    }

    /// <summary>
    /// Lists all the possible schema categories
    /// </summary>
    /// <returns></returns>
    public override DataTable GetSchema()
    {
        DataTable schemaCollections = new DataTable("MetaDataCollections");
        schemaCollections.Columns.Add(new DataColumn("CollectionName", typeof(string)));
        schemaCollections.Columns.Add(new DataColumn("NumberOfRestrictions", typeof(int)));
        schemaCollections.Columns.Add(new DataColumn("NumberOfIdentifierParts", typeof(int)));

        // Populate the table with information about each schema collection.
        schemaCollections.Rows.Add("Tables", 0, 0); // Example: No restrictions, adjust if needed
        schemaCollections.Rows.Add("Columns", 0, 0); // Example: No restrictions, adjust if needed

        // Add more collections as supported by your provider.
        // For example, Indexes, Views, Procedures, etc., if applicable.

        return schemaCollections;
    }

    public override DataTable GetSchema(string collectionName)
    {
        switch (collectionName.ToUpperInvariant())
        {
            case "TABLES":
                return GetTablesSchema();
            case "COLUMNS":
                return GetColumnsSchema();
            default:
                throw new ArgumentException("Unsupported schema collection");
        }
    }


    /// <summary>
    /// Disposes the connection.
    /// </summary>
    protected new void Dispose()
    {
        base.Dispose();
        state = ConnectionState.Closed;
    }

    private DataTable GetTablesSchema()
    {
        string query = "SELECT TABLE_NAME, TABLE_TYPE FROM INFORMATION_SCHEMA.TABLES";
        return ExecuteQuery(query);
    }

    private DataTable GetColumnsSchema()
    {
        string query = "SELECT TABLE_NAME, COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS";
        return ExecuteQuery(query);
    }

    private DataTable ExecuteQuery(string query)
    {
        DataTable schemaTable = new DataTable();
        using (var command = CreateCommand(query))
        {
            command.Connection = this;
            using (var reader = command.ExecuteReader())
            {
                schemaTable.Load(reader);
            }
        }
        return schemaTable;
    }
}
