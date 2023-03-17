namespace System.Data.FileClient;

public abstract class FileConnection : IDbConnection, IConnectionStringProperties
{
    private readonly FileConnectionString connectionString = new();
    private ConnectionState state;
    public abstract string FileExtension { get; }
    public string? ConnectionString { get => connectionString.ConnectionString!; set => connectionString.Parse(value); }
    public string? DataSource => connectionString.DataSource;
    public bool Formatted => connectionString.Formatted;

    public int ConnectionTimeout { get; }
    public string Database => connectionString.DataSource ?? string.Empty;
    public ConnectionState State => state;
 
    protected FileConnection(string connectionString)
    {
        ArgumentNullException.ThrowIfNull(connectionString, nameof(connectionString));
        this.connectionString.Parse(connectionString);
        state = ConnectionState.Closed;
    }


  
    public PathType PathType => this.GetPathType();

    public FileReader FileReader { get; protected set; }

    public abstract IDbTransaction BeginTransaction();

    public abstract IDbTransaction BeginTransaction(IsolationLevel il);

 
    public void ChangeDatabase(string databaseName)
    {
        ArgumentNullException.ThrowIfNull(nameof(databaseName));
        ThrowHelper.ThrowIfInvalidPath(PathType);
        
    }

    public void Close() => state = ConnectionState.Closed;

    public abstract IDbCommand CreateCommand();
  


    public void Open()
    {
        ThrowHelper.ThrowIfInvalidPath(PathType);
        state = ConnectionState.Open;
    }

    public virtual void Dispose()
    {
        state = ConnectionState.Closed;
      
        
    }

}