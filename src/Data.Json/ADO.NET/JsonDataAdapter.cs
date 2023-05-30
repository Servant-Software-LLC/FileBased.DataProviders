namespace System.Data.JsonClient;

public class JsonDataAdapter : FileDataAdapter<JsonParameter>
{
    public JsonDataAdapter()
    {
    }

    public JsonDataAdapter(JsonCommand selectCommand) : base(selectCommand)
    {
    }

    public JsonDataAdapter(string query, JsonConnection connection) : base(query, connection)
    {
    }

    protected override FileWriter CreateWriter(FileStatement fileStatement) => fileStatement switch
    {
        global::Data.Common.FileStatements.FileDelete deleteStatement => new JsonDelete(deleteStatement, (JsonConnection)UpdateCommand!.Connection!, (FileCommand<JsonParameter>)UpdateCommand),
        FileInsert insertStatement => new JsonInsert(insertStatement, (JsonConnection)UpdateCommand!.Connection!, (FileCommand<JsonParameter>)UpdateCommand),
        FileUpdate updateStatement => new JsonUpdate(updateStatement, (JsonConnection)UpdateCommand!.Connection!, (FileCommand<JsonParameter>)UpdateCommand),

        _ => throw new InvalidOperationException("query not supported")
    };
}