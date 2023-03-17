namespace System.Data.XmlClient;
public class XmlTransaction : FileTransaction
{
    public XmlTransaction(FileConnection connection, IsolationLevel isolationLevel = 0) 
        : base(connection, isolationLevel)
    {
    }
}