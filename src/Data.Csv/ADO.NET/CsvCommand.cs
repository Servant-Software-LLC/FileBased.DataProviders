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

    protected override FileDataReader CreateDataReader(FileQuery queryParser) => new CsvDataReader(queryParser, ((CsvConnection)Connection!).FileReader);

    public override IDbDataParameter CreateParameter() => new CsvParameter();
}