using Data.Json.Enum;

namespace System.Data.JsonClient;

public class JsonConnection : IDbConnection
{
    private string connectionString;
    private ConnectionState state;
    private JsonDocument? database;
    public string ConnectionString { get => connectionString; set => connectionString = value; }
    public int ConnectionTimeout { get; }
    public string Database => connectionString;
    public ConnectionState State => state;
 
    public JsonConnection(string connectionString)
    {
        ArgumentNullException.ThrowIfNull(nameof(connectionString));
        this.connectionString = connectionString.Split('=')[1].TrimEnd(';');
        state = ConnectionState.Closed;
        JsonReader = new JsonReader(this);
    }

    internal JsonReader JsonReader { get; private set; }
    public PathType PathType { get; private set; }

    public IDbTransaction BeginTransaction()
    {
        throw new NotImplementedException();
    }

    public IDbTransaction BeginTransaction(IsolationLevel il)
    {
        throw new NotImplementedException();
    }

    public void ChangeDatabase(string databaseName)
    {
        PathType = this.GetPathType();
        ArgumentNullException.ThrowIfNull(nameof(databaseName));
        ThrowHelper.ThrowIfInvalidPath(PathType);
        using var file = File.OpenRead(databaseName);
        database = JsonDocument.Parse(file);
    }

    public void Close()
    {
        state = ConnectionState.Closed;
    }


    public IDbCommand CreateCommand()
    {
            return new JsonCommand(this);
    }
    
    
    public void Open()
    {
        PathType = this.GetPathType();
        ThrowHelper.ThrowIfInvalidPath(PathType);
        state = ConnectionState.Open;
    }

    public void Dispose()
    {
        state = ConnectionState.Closed;
        database?.Dispose();
        JsonReader.Dispose();
        
    }
}