namespace System.Data.CsvClient;

public class CsvCommand : FileCommand
{
    public CsvCommand()
    {
    }

    public CsvCommand(string command) : base(command)
    {
    }

    public CsvCommand(CsvConnection connection) : base(connection)
    {
    }

    public CsvCommand(string cmdText, CsvConnection connection) : base(cmdText, connection)
    {
    }

    public CsvCommand(string cmdText, CsvConnection connection, CsvTransaction transaction) 
        : base(cmdText, connection, transaction)
    {
    }

    public override CsvParameter CreateParameter() => new();
    public override CsvParameter CreateParameter(string parameterName, object value) => new(parameterName, value);

    public override CsvDataAdapter CreateAdapter() => new(this);

    protected override FileWriter CreateWriter(FileQuery queryParser) => queryParser switch
    {
        FileDeleteQuery deleteQuery =>
            new CsvDelete(deleteQuery, (CsvConnection)Connection!, this),
        FileInsertQuery insertQuery =>
            new CsvInsert(insertQuery, (CsvConnection)Connection!, this),
        FileUpdateQuery updateQuery =>
            new CsvUpdate(updateQuery, (CsvConnection)Connection!, this),

        _ => throw new InvalidOperationException("query not supported")
    };

    protected override CsvDataReader CreateDataReader(FileQuery queryParser) => new(queryParser, ((CsvConnection)Connection!).FileReader);

}