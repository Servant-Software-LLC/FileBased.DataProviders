namespace System.Data.CsvClient;
public class CsvDataAdapter : FileDataAdapter<CsvParameter>
{
    public CsvDataAdapter()
    {
    }

    public CsvDataAdapter(CsvCommand selectCommand) : base(selectCommand)
    {
    }

    public CsvDataAdapter(string query, CsvConnection connection) : base(query, connection)
    {
    }

    protected override FileWriter CreateWriter(FileStatement fileStatement) => fileStatement switch
    {
        global::Data.Common.FileStatements.FileDelete deleteStatement =>
            new CsvDelete(deleteStatement, (CsvConnection)UpdateCommand!.Connection!, (FileCommand<CsvParameter>)UpdateCommand),
        global::Data.Common.FileStatements.FileInsert insertStatement =>
            new CsvInsert(insertStatement, (CsvConnection)UpdateCommand!.Connection!, (FileCommand<CsvParameter>)UpdateCommand),
        global::Data.Common.FileStatements.FileUpdate updateStatement =>
            new CsvUpdate(updateStatement, (CsvConnection)UpdateCommand!.Connection!, (FileCommand<CsvParameter>)UpdateCommand),

        _ => throw new InvalidOperationException("query not supported")
    };
}