namespace System.Data.JsonClient;

public class JsonConnection : FileConnection
{
    public JsonConnection(string connectionString) : 
        base(connectionString)
    {
        FileReader = new JsonReader(this);
    }

    public override string FileExtension => "json";

    public override IDbTransaction BeginTransaction() => new JsonTransaction(this, default);

    public override IDbTransaction BeginTransaction(IsolationLevel il) => BeginTransaction();

    public override IDbCommand CreateCommand() => new JsonCommand(this);
    public override FileCommand CreateCommand(string cmdText) => new JsonCommand(cmdText, this);

}