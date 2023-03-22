namespace System.Data.CsvClient;

public class CsvConnection : FileConnection
{
    public CsvConnection(string connectionString) : 
        base(connectionString)
    {
        FileReader = new CsvReader(this);
    }


    public override string FileExtension => "csv";

    public override void Open()
    {
        base.Open();

        EnsureNotFileAsDatabase();
    }

    public override void ChangeDatabase(string constring)
    {
        base.ChangeDatabase(constring);
        EnsureNotFileAsDatabase();
    }
    private void EnsureNotFileAsDatabase()
    {
        if (PathType == PathType.File)
        {
            throw new InvalidOperationException("File as database is not supported in Csv Provider");

        }
    }

    public override IDbTransaction BeginTransaction() => new CsvTransaction(this, default);

    public override IDbTransaction BeginTransaction(IsolationLevel il) => BeginTransaction();

    public override IDbCommand CreateCommand() => new CsvCommand(this);
    public override FileCommand CreateCommand(string cmdText) => new CsvCommand(cmdText, this);
}