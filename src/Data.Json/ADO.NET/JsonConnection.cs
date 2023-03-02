using Data.Json.Enum;
using Data.Json.Interfaces;
using Data.Json.Utils.ConnectionString;

namespace System.Data.JsonClient;

public class JsonConnection : IDbConnection, IConnectionStringProperties
{
    private readonly JsonConnectionString connectionString = new();
    private ConnectionState state;
    private JsonDocument? database;

    public string? ConnectionString { get => connectionString.ConnectionString; set => connectionString.Parse(value); }
    public string? DataSource => connectionString.DataSource;
    public bool Formatted => connectionString.Formatted;

    public int ConnectionTimeout { get; }
    public string Database => connectionString.DataSource ?? string.Empty;
    public ConnectionState State => state;
 
    public JsonConnection(string connectionString)
    {
        ArgumentNullException.ThrowIfNull(connectionString, nameof(connectionString));
        this.connectionString.Parse(connectionString);


        state = ConnectionState.Closed;
        JsonReader = new JsonReader(this);
    }


    internal JsonReader JsonReader { get; private set; }
    public PathType PathType => this.GetPathType();


    public JsonTransaction BeginTransaction() => BeginTransaction(default);

    public JsonTransaction BeginTransaction(IsolationLevel il) => new JsonTransaction(this,il);

    IDbTransaction IDbConnection.BeginTransaction() => BeginTransaction();

    IDbTransaction IDbConnection.BeginTransaction(IsolationLevel il) => BeginTransaction(il);

    public void ChangeDatabase(string databaseName)
    {
        ArgumentNullException.ThrowIfNull(nameof(databaseName));
        ThrowHelper.ThrowIfInvalidPath(PathType);
        using var file = File.OpenRead(databaseName);
        database = JsonDocument.Parse(file);
    }

    public void Close() => state = ConnectionState.Closed;

    public JsonCommand CreateCommand() => new(this);

    IDbCommand IDbConnection.CreateCommand() => CreateCommand();

    public void Open()
    {
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