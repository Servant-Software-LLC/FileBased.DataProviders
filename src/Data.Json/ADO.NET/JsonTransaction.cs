namespace System.Data.JsonClient;
public class JsonTransaction : FileTransaction
{
    public JsonTransaction(FileConnection connection, IsolationLevel isolationLevel = 0) 
        : base(connection, isolationLevel)
    {
    }
}