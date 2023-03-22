namespace System.Data.XmlClient;

public class XmlConnection : FileConnection
{
    public XmlConnection(string connectionString) : 
        base(connectionString)
    {
        FileReader = new XmlReader(this);
    }


    public override string FileExtension => "xml";

    public override XmlTransaction BeginTransaction() => new(this, default);

    public override XmlTransaction BeginTransaction(IsolationLevel il) => BeginTransaction();
    public override XmlDataAdapter CreateDataAdapter(string query) => new(query, this);

    public override XmlCommand CreateCommand() => new(this);
    public override XmlCommand CreateCommand(string cmdText) => new(cmdText, this);

}