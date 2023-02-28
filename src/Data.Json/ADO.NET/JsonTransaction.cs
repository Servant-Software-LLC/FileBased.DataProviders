namespace System.Data.JsonClient;

public class JsonTransaction : IDbTransaction
{
    private JsonConnection _connection;
    private IsolationLevel _isolationLevel;
    internal bool TransactionDone = false;
    internal readonly List<JsonWriter> Writers
        =new List<JsonWriter>();


    internal JsonTransaction(JsonConnection connection, IsolationLevel isolationLevel = default)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        _isolationLevel = isolationLevel;
    }

    public void Commit()
    {
        if (TransactionDone)
        {
            throw new InvalidOperationException("This JsonTransaction has completed; it is no longer usable");
        }
        TransactionDone = true;
        Writers.ForEach(writer =>
        {
            writer.Execute();
        });

    }

    public void Rollback()
    {
        //We don't need to do anything as we haven't saved the data to database
        if (TransactionDone)
        {
            throw new InvalidOperationException("This JsonTransaction has completed; it is no longer usable");
        }
        TransactionDone = true;
    }

    public void Dispose()
    {
        Writers.Clear();
    }

    public IDbConnection Connection => _connection;

    public IsolationLevel IsolationLevel => _isolationLevel;
}