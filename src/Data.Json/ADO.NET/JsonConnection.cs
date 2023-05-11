using System.Data.Common;

namespace System.Data.JsonClient;

public class JsonConnection : FileConnection<JsonParameter>
{
    public JsonConnection(string connectionString) : 
        base(connectionString)
    {
        FileReader = new JsonReader(this);
    }

    public override string FileExtension => "json";

    public override JsonTransaction BeginTransaction() => new(this, default);

    public override JsonTransaction BeginTransaction(IsolationLevel il) => BeginTransaction();
    protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) => BeginTransaction(isolationLevel);

    public override JsonDataAdapter CreateDataAdapter(string query) => new(query, this);

    public override JsonCommand CreateCommand() => new(this);
    public override JsonCommand CreateCommand(string cmdText) => new(cmdText, this);

}