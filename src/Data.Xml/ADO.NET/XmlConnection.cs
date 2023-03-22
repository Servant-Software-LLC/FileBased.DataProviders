namespace System.Data.XmlClient;

public class XmlConnection : FileConnection
{
    public XmlConnection(string connectionString) : 
        base(connectionString)
    {
        FileReader = new XmlReader(this);
    }


    public override string FileExtension => "xml";

    public override IDbTransaction BeginTransaction() => new XmlTransaction(this, default);

    public override IDbTransaction BeginTransaction(IsolationLevel il) => BeginTransaction();

    public override IDbCommand CreateCommand() => new XmlCommand(this);
    public override FileCommand CreateCommand(string cmdText) => new XmlCommand(cmdText, this);

}