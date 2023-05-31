namespace System.Data.XmlClient;

public class XmlDataAdapter : FileDataAdapter<XmlParameter>
{
    public XmlDataAdapter()
    {
    }

    public XmlDataAdapter(XmlCommand selectCommand) : base(selectCommand)
    {
    }

    public XmlDataAdapter(string query, XmlConnection connection) : base(query, connection)
    {
    }

    protected override FileWriter CreateWriter(FileStatement fileStatement) => fileStatement switch
    {
        global::Data.Common.FileStatements.FileDelete deleteStatement => new XmlDelete(deleteStatement, (XmlConnection)UpdateCommand!.Connection!, (FileCommand<XmlParameter>)UpdateCommand),
        global::Data.Common.FileStatements.FileInsert insertStatement => new XmlInsert(insertStatement, (XmlConnection)UpdateCommand!.Connection!, (FileCommand<XmlParameter>)UpdateCommand),
        global::Data.Common.FileStatements.FileUpdate updateStatement => new XmlUpdate(updateStatement, (XmlConnection)UpdateCommand!.Connection!, (FileCommand<XmlParameter>)UpdateCommand),

        _ => throw new InvalidOperationException("query not supported")
    };
}