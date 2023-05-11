namespace System.Data.CsvClient;

public class CsvTransaction : FileTransaction<CsvParameter>
{
    private readonly CsvConnection connection;

    public CsvTransaction(CsvConnection connection, IsolationLevel isolationLevel = 0) 
        : base(connection, isolationLevel)
    {
        this.connection = connection;
    }

    public override CsvCommand CreateCommand(string cmdText) => new(cmdText, connection, this);
}