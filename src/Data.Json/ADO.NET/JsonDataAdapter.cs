namespace System.Data.JsonClient;

public class JsonDataAdapter : FileDataAdapter
{
    public JsonDataAdapter()
    {
    }

    public JsonDataAdapter(FileCommand selectCommand) : base(selectCommand)
    {
    }

    public JsonDataAdapter(string query, FileConnection connection) : base(query, connection)
    {
    }

    protected override FileWriter CreateWriter(FileQuery queryParser) => queryParser switch
    {
        FileDeleteQuery deleteQuery =>
            new JsonDelete(deleteQuery, (JsonConnection)UpdateCommand!.Connection!, (FileCommand)UpdateCommand),
        FileInsertQuery insertQuery =>
            new JsonInsert(insertQuery, (JsonConnection)UpdateCommand!.Connection!, (FileCommand)UpdateCommand),
        FileUpdateQuery updateQuery =>
            new JsonUpdate(updateQuery, (JsonConnection)UpdateCommand!.Connection!, (FileCommand)UpdateCommand),

        _ => throw new InvalidOperationException("query not supported")
    };
}