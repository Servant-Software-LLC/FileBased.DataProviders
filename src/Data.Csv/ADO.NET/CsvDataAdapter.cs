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
    protected override FileWriter<CsvParameter> CreateWriter(FileQuery<CsvParameter> queryParser) => queryParser switch
    {
        FileDeleteQuery<CsvParameter> deleteQuery =>
            new CsvDelete(deleteQuery, (CsvConnection)UpdateCommand!.Connection!, (FileCommand<CsvParameter>)UpdateCommand),
        FileInsertQuery<CsvParameter> insertQuery =>
            new CsvInsert(insertQuery, (CsvConnection)UpdateCommand!.Connection!, (FileCommand<CsvParameter>)UpdateCommand),
        FileUpdateQuery<CsvParameter> updateQuery =>
            new CsvUpdate(updateQuery, (CsvConnection)UpdateCommand!.Connection!, (FileCommand<CsvParameter>)UpdateCommand),

        _ => throw new InvalidOperationException("query not supported")
    };
}