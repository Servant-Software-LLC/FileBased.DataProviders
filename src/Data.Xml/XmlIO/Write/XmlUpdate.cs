namespace Data.Xml.XmlIO.Write;

internal class XmlUpdate : FileUpdate
{
    public XmlUpdate(FileUpdateQuery queryParser, FileConnection FileConnection, FileCommand FileCommand) 
        : base(queryParser, FileConnection, FileCommand)
    {
        dataSetWriter = new XmlDataSetWriter(FileConnection, queryParser);
    }
}
