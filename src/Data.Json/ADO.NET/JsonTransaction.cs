namespace System.Data.JsonClient;
public class JsonTransaction : FileTransaction<JsonParameter>
{
    private readonly JsonConnection connection;

    public JsonTransaction(JsonConnection connection, IsolationLevel isolationLevel = 0) 
        : base(connection, isolationLevel)
    {
        this.connection = connection;
    }

    public override JsonCommand CreateCommand(string cmdText) => new(cmdText, connection, this);
}