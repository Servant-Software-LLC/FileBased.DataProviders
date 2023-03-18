namespace System.Data.XmlClient;
public class XmlDataAdapter : FileDataAdapter
{
    public XmlDataAdapter()
    {
    }

    public XmlDataAdapter(FileCommand selectCommand) : base(selectCommand)
    {
    }

    public XmlDataAdapter(string query, FileConnection connection) : base(query, connection)
    {
    }
    protected override FileWriter CreateWriter(FileQuery queryParser) => queryParser switch
    {
        FileDeleteQuery deleteQuery =>
        new XmlDelete(deleteQuery, (XmlConnection)UpdateCommand!.Connection!, (FileCommand)UpdateCommand),
        FileInsertQuery insertQuery =>
        new XmlInsert(insertQuery, (XmlConnection)UpdateCommand!.Connection!, (FileCommand)UpdateCommand),
        FileUpdateQuery updateQuery =>
      new XmlUpdate(updateQuery, (XmlConnection)UpdateCommand!.Connection!, (FileCommand)UpdateCommand),
        _ => throw new InvalidOperationException("query not supported")
    };
}