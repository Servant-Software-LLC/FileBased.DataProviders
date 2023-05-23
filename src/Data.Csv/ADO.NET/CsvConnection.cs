namespace System.Data.CsvClient;

public class CsvConnection : FileConnection<CsvParameter>
{
    public CsvConnection(string connectionString) : 
        base(connectionString)
    {
        FileReader = !AdminMode ? new CsvReader(this) : null;
    }


    public override string FileExtension => "csv";

    public override void Open()
    {
        base.Open();

        EnsureNotFileAsDatabase();
    }

    public override void ChangeDatabase(string connString)
    {
        base.ChangeDatabase(connString);
        EnsureNotFileAsDatabase();
    }

    private void EnsureNotFileAsDatabase()
    {
        if (PathType == PathType.File)
        {
            throw new InvalidOperationException("File as database is not supported in Csv Provider");

        }
    }

    public override CsvTransaction BeginTransaction() => new(this, default);

    public override CsvTransaction BeginTransaction(IsolationLevel il) => BeginTransaction();

    public override CsvDataAdapter CreateDataAdapter(string query) => new(query, this);

    public override CsvCommand CreateCommand() => new(this);
    public override CsvCommand CreateCommand(string cmdText) => new(cmdText, this);

    protected override void CreateFileAsDatabase(string databaseFileName) =>
        File.WriteAllText(databaseFileName, string.Empty);

}