using Data.Json.JsonIO.Read;
using Irony.Parsing.Construction;

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
        try
        {
            //as we have modified the json file so we don't need to update the tables
            Writers.ForEach(writer =>
            {
                writer.Execute();
            });
        }
        finally
        {
        }
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

    }

    public IDbConnection Connection => _connection;

    public IsolationLevel IsolationLevel => _isolationLevel;
}