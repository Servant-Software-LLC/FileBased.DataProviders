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

    protected override FileWriter CreateWriter(FileStatement fileStatement) => fileStatement switch
    {
        FileDelete deleteStatement => new JsonDelete(deleteStatement, (JsonConnection)Connection!, this),
        FileInsert insertStatement => new JsonInsert(insertStatement, (JsonConnection)Connection!, this),
        FileUpdate updateStatement => new JsonUpdate(updateStatement, (JsonConnection)Connection!, this),

        _ => throw new InvalidOperationException("query not supported")
    };

    protected override JsonDataReader CreateDataReader(IEnumerable<FileStatement> fileStatements) => 
        new(fileStatements, ((JsonConnection)Connection!).FileReader, CreateWriter);

}