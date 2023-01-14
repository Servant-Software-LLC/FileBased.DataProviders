using System.Data;
using System.Text.Json;

namespace Data.Json;

public class JsonConnection : IDbConnection
{
    public JsonConnection()
    {
        State = ConnectionState.Closed;
    }

    public JsonConnection(JsonDocument jsonDocument)
        :this($"Data Source=:memory:;Json={jsonDocument}")
	{
    }

	public JsonConnection(string connectionString)
	{
        ConnectionString = !string.IsNullOrEmpty(connectionString) ? connectionString : throw new ArgumentNullException(nameof(connectionString));
        State = ConnectionState.Closed;
	}

    public string ConnectionString { get; set; }

    public int ConnectionTimeout => throw new NotImplementedException();

    public string Database => throw new NotImplementedException();

    public ConnectionState State { get; private set; }

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
        throw new NotImplementedException();
    }

    public void Close()
    {
        throw new NotImplementedException();
    }

    public IDbCommand CreateCommand()
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public void Open()
    {
        throw new NotImplementedException();
    }
}
