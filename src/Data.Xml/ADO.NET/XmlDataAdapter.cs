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

    protected override FileWriter<XmlParameter> CreateWriter(FileQuery<XmlParameter> queryParser) => queryParser switch
    {
        FileDeleteQuery<XmlParameter> deleteQuery => new XmlDelete(deleteQuery, (XmlConnection)UpdateCommand!.Connection!, (FileCommand<XmlParameter>)UpdateCommand),
        FileInsertQuery<XmlParameter> insertQuery => new XmlInsert(insertQuery, (XmlConnection)UpdateCommand!.Connection!, (FileCommand<XmlParameter>)UpdateCommand),
        FileUpdateQuery<XmlParameter> updateQuery => new XmlUpdate(updateQuery, (XmlConnection)UpdateCommand!.Connection!, (FileCommand<XmlParameter>)UpdateCommand),

        _ => throw new InvalidOperationException("query not supported")
    };
}