namespace System.Data.XmlClient;

public class XmlTransaction : FileTransaction
{
    private readonly XmlConnection connection;

    public XmlTransaction(XmlConnection connection, IsolationLevel isolationLevel = 0) 
        : base(connection, isolationLevel)
    {
        this.connection = connection;
    }

    public override XmlCommand CreateCommand(string cmdText) => new(cmdText, connection, this);
}