namespace Data.Xml.XmlIO.Delete;

internal class XmlDelete : FileDelete
{
    public XmlDelete(FileDeleteQuery queryParser, FileConnection connection, FileCommand jsonCommand) 
        : base(queryParser, connection, jsonCommand)
    {
        dataSetWriter = new XmlDataSetWriter(connection,queryParser);
    }
}

