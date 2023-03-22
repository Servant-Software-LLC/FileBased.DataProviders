namespace System.Data.FileClient;

public abstract class FileConnection : IDbConnection, IConnectionStringProperties
{
    private readonly FileConnectionString connectionString = new();

    public abstract string FileExtension { get; }
    public string? ConnectionString { get => connectionString.ConnectionString!; set => connectionString.Parse(value); }
    public string? DataSource => connectionString.DataSource;
    public bool Formatted => connectionString.Formatted;

    public int ConnectionTimeout { get; }
    public string Database => connectionString.DataSource ?? string.Empty;
    public ConnectionState State { get; private set; }
 
    protected FileConnection(string connectionString)
    {
        ArgumentNullException.ThrowIfNull(connectionString, nameof(connectionString));
        this.connectionString.Parse(connectionString);
        State = ConnectionState.Closed;
    }


 
    public PathType PathType => this.GetPathType();

    public FileReader FileReader { get; protected set; }

    public abstract FileTransaction BeginTransaction();

    public abstract FileTransaction BeginTransaction(IsolationLevel il);

    IDbTransaction IDbConnection.BeginTransaction() => BeginTransaction();

    IDbTransaction IDbConnection.BeginTransaction(IsolationLevel il) => BeginTransaction(il);

    public abstract FileDataAdapter CreateDataAdapter(string query);

    public virtual void ChangeDatabase(string databaseName)
    {
        ArgumentNullException.ThrowIfNull(nameof(databaseName));
        ThrowHelper.ThrowIfInvalidPath(PathType);
        
    }

    public void Close() => State = ConnectionState.Closed;

    public abstract FileCommand CreateCommand();
    public abstract FileCommand CreateCommand(string cmdText);
    IDbCommand IDbConnection.CreateCommand() => CreateCommand();


    public virtual void Open()
    {
        ThrowHelper.ThrowIfInvalidPath(PathType);
        State = ConnectionState.Open;
    }

    public virtual void Dispose()
    {
        State = ConnectionState.Closed;
    }

}