using Data.Common.Utils;
using Microsoft.Extensions.Logging;

namespace System.Data.FileClient;

public abstract class FileConnection<TFileParameter> : DbConnection, IFileConnection, IConnectionStringProperties
    where TFileParameter : FileParameter<TFileParameter>, new()
{
    private readonly FileConnectionString connectionString = new();
    private ConnectionState state;

    public abstract string FileExtension { get; }
    public override string ConnectionString { get => connectionString.ConnectionString!; set => connectionString.ConnectionString = value; }
    public override string DataSource => connectionString.DataSource;
    public bool? Formatted => connectionString.Formatted;
    public LogLevel LogLevel => connectionString.LogLevel ?? LogLevel.None;
    public bool CreateIfNotExist => connectionString.CreateIfNotExist ?? false;

    public override string Database => connectionString.DataSource ?? string.Empty;
    public override ConnectionState State => state;
    
    internal LoggerServices LoggerServices { get; }
    LoggerServices IFileConnection.LoggerServices => LoggerServices;
    private ILogger<FileConnection<TFileParameter>> log => LoggerServices.CreateLogger<FileConnection<TFileParameter>>();

    protected FileConnection(string connectionString)
    {
        ArgumentNullException.ThrowIfNull(connectionString, nameof(connectionString));
        this.connectionString = connectionString;
        state = ConnectionState.Closed;

        LoggerServices = new LoggerServices(LogLevel);
        log.LogInformation($"{GetType()} created.  ConnectionString = {connectionString}");    
    }

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

    public PathType PathType => this.GetPathType();
    public bool FolderAsDatabase => PathType == PathType.Directory;
    public bool AdminMode => PathType == PathType.Admin;

    public FileReader FileReader { get; protected set; }

    public bool CaseInsensitive => true;

    public new abstract FileTransaction<TFileParameter> BeginTransaction();

    public new abstract FileTransaction<TFileParameter> BeginTransaction(IsolationLevel il);

    IDbTransaction IDbConnection.BeginTransaction() => BeginTransaction();

    IDbTransaction IDbConnection.BeginTransaction(IsolationLevel il) => BeginTransaction(il);

    protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) => BeginTransaction(isolationLevel);

    public abstract FileDataAdapter<TFileParameter> CreateDataAdapter(string query);

    public override void ChangeDatabase(string databaseName)
    {
        ArgumentNullException.ThrowIfNull(nameof(databaseName));
        connectionString.DataSource = databaseName;

        //Other ADO.NET providers that support automatically creating a database when provided in
        //the connection string, either still require that for this operation that the database
        //already exists (SQL Server LocalDB) or just throws a NotSupportedException regardless
        //of whether the database exists or not (SQLite).
        //Therefore, we will not automatically create the database if it doesn't exist and throw.
        ThrowHelper.ThrowIfInvalidPath(PathType, databaseName);        
    }

    public override void Close() => state = ConnectionState.Closed;

    public new abstract FileCommand<TFileParameter> CreateCommand();
    public abstract FileCommand<TFileParameter> CreateCommand(string cmdText);
    IDbCommand IDbConnection.CreateCommand() => CreateCommand();

    protected override DbCommand CreateDbCommand() => CreateCommand();

    protected internal abstract void CreateFileAsDatabase(string databaseFileName);

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