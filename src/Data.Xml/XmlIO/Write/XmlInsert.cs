namespace Data.Xml.XmlIO.Write;

internal class XmlInsert : FileInsert
{
    public XmlInsert(FileInsertQuery queryParser, FileConnection jsonConnection, FileCommand jsonCommand) 
        : base(queryParser, jsonConnection, jsonCommand)
    {
        dataSetWriter = new XmlDataSetWriter(jsonConnection, queryParser);
    }
}
