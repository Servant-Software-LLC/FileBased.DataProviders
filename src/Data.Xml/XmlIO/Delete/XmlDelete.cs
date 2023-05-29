using System.Data.XmlClient;

namespace Data.Xml.XmlIO.Delete;

internal class XmlDelete : FileDelete
{
    public XmlDelete(FileDeleteQuery queryParser, FileConnection<XmlParameter> connection, FileCommand<XmlParameter> command) 
        : base(queryParser, connection, command)
    {
        dataSetWriter = new XmlDataSetWriter(connection, queryParser);
    }
}

