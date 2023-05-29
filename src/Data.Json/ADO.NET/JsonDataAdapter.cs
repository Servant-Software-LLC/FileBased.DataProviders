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

    protected override FileWriter CreateWriter(FileQuery queryParser) => queryParser switch
    {
        FileDeleteQuery deleteQuery => new JsonDelete(deleteQuery, (JsonConnection)UpdateCommand!.Connection!, (FileCommand<JsonParameter>)UpdateCommand),
        FileInsertQuery insertQuery => new JsonInsert(insertQuery, (JsonConnection)UpdateCommand!.Connection!, (FileCommand<JsonParameter>)UpdateCommand),
        FileUpdateQuery updateQuery => new JsonUpdate(updateQuery, (JsonConnection)UpdateCommand!.Connection!, (FileCommand<JsonParameter>)UpdateCommand),

        _ => throw new InvalidOperationException("query not supported")
    };
}