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

    protected override FileWriter<JsonParameter> CreateWriter(FileQuery<JsonParameter> queryParser) => queryParser switch
    {
        FileDeleteQuery<JsonParameter> deleteQuery => new JsonDelete(deleteQuery, (JsonConnection)UpdateCommand!.Connection!, (FileCommand<JsonParameter>)UpdateCommand),
        FileInsertQuery<JsonParameter> insertQuery => new JsonInsert(insertQuery, (JsonConnection)UpdateCommand!.Connection!, (FileCommand<JsonParameter>)UpdateCommand),
        FileUpdateQuery<JsonParameter> updateQuery => new JsonUpdate(updateQuery, (JsonConnection)UpdateCommand!.Connection!, (FileCommand<JsonParameter>)UpdateCommand),

        _ => throw new InvalidOperationException("query not supported")
    };
}