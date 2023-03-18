namespace System.Data.XmlClient;

public class XmlCommand : FileCommand
{
    public XmlCommand()
    {
    }

    public XmlCommand(string command) : base(command)
    {
    }

    public XmlCommand(XmlConnection connection) : base(connection)
    {
    }

    public XmlCommand(string cmdText, XmlConnection connection) : base(cmdText, connection)
    {
    }

    public XmlCommand(string cmdText, XmlConnection connection, XmlTransaction transaction) : base(cmdText, connection, transaction)
    {
    }

    protected override FileWriter CreateWriter(FileQuery queryParser) => queryParser switch
    {
        FileDeleteQuery deleteQuery =>
        new XmlDelete(deleteQuery, (XmlConnection)Connection!, this),
        FileInsertQuery insertQuery =>
        new XmlInsert(insertQuery, (XmlConnection)Connection!, this),
        FileUpdateQuery updateQuery =>
      new XmlUpdate(updateQuery, (XmlConnection)Connection!, this),
        _ => throw new InvalidOperationException("query not supported")
    };

    protected override FileDataReader CreateDataReader(FileQuery queryParser)
    {
        return new XmlDataReader(queryParser, ((XmlConnection)Connection!).FileReader);
    }

    public override IDbDataParameter CreateParameter()
    {
        return new XmlParameter();
    }
}