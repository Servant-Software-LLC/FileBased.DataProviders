namespace System.Data.JsonClient;

public class JsonCommand : FileCommand<JsonParameter>
{
    public JsonCommand()
    {
    }

    public JsonCommand(string command) : base(command)
    {
    }

    public JsonCommand(JsonConnection connection) : base(connection)
    {
    }

    public JsonCommand(string cmdText, JsonConnection connection) : base(cmdText, connection)
    {
    }

    public JsonCommand(string cmdText, JsonConnection connection, JsonTransaction transaction) 
        : base(cmdText, connection, transaction)
    {
    }

    public override JsonParameter CreateParameter() => new();
    public override JsonParameter CreateParameter(string parameterName, object value) => new(parameterName, value);

    public override JsonDataAdapter CreateAdapter() => new(this);

    protected override FileWriter CreateWriter(FileQuery queryParser) => queryParser switch
    {
        FileDeleteQuery deleteQuery => new JsonDelete(deleteQuery, (JsonConnection)Connection!, this),
        FileInsertQuery insertQuery => new JsonInsert(insertQuery, (JsonConnection)Connection!, this),
        FileUpdateQuery updateQuery => new JsonUpdate(updateQuery, (JsonConnection)Connection!, this),

        _ => throw new InvalidOperationException("query not supported")
    };

    protected override JsonDataReader CreateDataReader(IEnumerable<FileQuery> queryParser) => 
        new(queryParser, ((JsonConnection)Connection!).FileReader);

}