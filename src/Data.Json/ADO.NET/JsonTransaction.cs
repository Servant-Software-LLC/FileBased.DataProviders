namespace System.Data.JsonClient;

public class JsonTransaction : IDbTransaction
{
    private JsonConnection _connection;
    private IsolationLevel _isolationLevel;
    private string _tempFilePath;

    public JsonTransaction(JsonConnection connection, IsolationLevel isolationLevel)
    {
        _connection = connection;
        _isolationLevel = isolationLevel;
        _tempFilePath = Path.GetTempFileName();
        File.Copy(_connection.Database, _tempFilePath);
    }

    public void Commit()
    {
        File.Delete(_connection.Database);
        File.Move(_tempFilePath, _connection.Database);
    }

    public void Rollback()
    {
        File.Delete(_connection.Database);
        File.Move(_tempFilePath, _connection.Database);
    }

    public void Dispose()
    {
        File.Delete(_tempFilePath);
    }

    public IDbConnection Connection => _connection;

    public IsolationLevel IsolationLevel => _isolationLevel;
}