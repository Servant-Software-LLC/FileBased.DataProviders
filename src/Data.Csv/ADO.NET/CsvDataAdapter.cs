namespace System.Data.XmlClient;
public class CsvDataAdapter : FileDataAdapter
{
    public CsvDataAdapter()
    {
    }

    public CsvDataAdapter(FileCommand selectCommand) : base(selectCommand)
    {
    }

    public CsvDataAdapter(string query, FileConnection connection) : base(query, connection)
    {
    }
    protected override FileWriter CreateWriter(FileQuery queryParser) => queryParser switch
    {
        FileDeleteQuery deleteQuery =>
            new CsvDelete(deleteQuery, (CsvConnection)UpdateCommand!.Connection!, (FileCommand)UpdateCommand),
        FileInsertQuery insertQuery =>
            new CsvInsert(insertQuery, (CsvConnection)UpdateCommand!.Connection!, (FileCommand)UpdateCommand),
        FileUpdateQuery updateQuery =>
            new CsvUpdate(updateQuery, (CsvConnection)UpdateCommand!.Connection!, (FileCommand)UpdateCommand),

        _ => throw new InvalidOperationException("query not supported")
    };
}