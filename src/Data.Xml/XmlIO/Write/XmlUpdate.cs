using System.Data.XmlClient;

namespace Data.Xml.XmlIO.Write;

internal class XmlUpdate : FileUpdate
{
    public XmlUpdate(FileUpdateQuery queryParser, FileConnection<XmlParameter> FileConnection, FileCommand<XmlParameter> FileCommand) 
        : base(queryParser, FileConnection, FileCommand)
    {
        dataSetWriter = new XmlDataSetWriter(FileConnection, queryParser);
    }
}
