namespace System.Data.CsvClient;

public class CsvTransaction : FileTransaction
{
    public CsvTransaction(FileConnection connection, IsolationLevel isolationLevel = 0) 
        : base(connection, isolationLevel)
    {
    }
}