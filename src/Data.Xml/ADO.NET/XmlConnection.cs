namespace System.Data.XmlClient;

public class XmlConnection : FileConnection
{
    public XmlConnection(string connectionString) : 
        base(connectionString)
    {
        FileReader = new XmlReader(this);

    }


    public override string FileExtension
    {
        get
        {
            return "xml";
        }
    }

    public override IDbTransaction BeginTransaction()
    {
        return new XmlTransaction(this, default);
    }

    public override IDbTransaction BeginTransaction(IsolationLevel il) => BeginTransaction();

    public override IDbCommand CreateCommand()
    {
        return new XmlCommand();
    }

   
}