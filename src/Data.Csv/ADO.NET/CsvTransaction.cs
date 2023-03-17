namespace System.Data.XmlClient;
public class CsvTransaction : FileTransaction
{
    public CsvTransaction(FileConnection connection, IsolationLevel isolationLevel = 0) 
        : base(connection, isolationLevel)
    {
    }
}