namespace System.Data.XmlClient;

public class XmlCommand : FileCommand<XmlParameter>
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

    public XmlCommand(string cmdText, XmlConnection connection, XmlTransaction transaction) 
        : base(cmdText, connection, transaction)
    {
    }

    public override XmlParameter CreateParameter() => new();
    public override XmlParameter CreateParameter(string parameterName, object value) => new(parameterName, value);

    public override XmlDataAdapter CreateAdapter() => new(this);

    protected override FileWriter CreateWriter(FileQuery queryParser) => queryParser switch
    {
        FileDeleteQuery deleteQuery => new XmlDelete(deleteQuery, (XmlConnection)Connection!, this),
        FileInsertQuery insertQuery => new XmlInsert(insertQuery, (XmlConnection)Connection!, this),
        FileUpdateQuery updateQuery => new XmlUpdate(updateQuery, (XmlConnection)Connection!, this),

        _ => throw new InvalidOperationException("query not supported")
    };

    protected override XmlDataReader CreateDataReader(IEnumerable<FileQuery> queryParser) => 
        new(queryParser, ((XmlConnection)Connection!).FileReader);

}