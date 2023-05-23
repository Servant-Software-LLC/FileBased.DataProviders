namespace System.Data.FileClient;

public abstract class FileConnection<TFileParameter> : DbConnection, IDbConnection, IConnectionStringProperties
    where TFileParameter : FileParameter<TFileParameter>, new()
{
    private readonly FileConnectionString connectionString = new();
    private ConnectionState state;

    public abstract string FileExtension { get; }
    public override string ConnectionString { get => connectionString.ConnectionString!; set => connectionString.ConnectionString = value; }
    public override string DataSource => connectionString.DataSource;
    public bool? Formatted => connectionString.Formatted;

    public override string Database => connectionString.DataSource ?? string.Empty;
    public override ConnectionState State => state;


    protected FileConnection(string connectionString)
    {
        ArgumentNullException.ThrowIfNull(connectionString, nameof(connectionString));
        this.connectionString = connectionString;
        state = ConnectionState.Closed;
    }

    public override string ServerVersion
    {
        get
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;
            return version;
        }
    }

    public PathType PathType => this.GetPathType();
    public bool FolderAsDatabase => PathType == PathType.Directory;
    public bool AdminMode => PathType == PathType.Admin;

    public FileReader<TFileParameter> FileReader { get; protected set; }

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
        ThrowHelper.ThrowIfInvalidPath(PathType, Database);
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