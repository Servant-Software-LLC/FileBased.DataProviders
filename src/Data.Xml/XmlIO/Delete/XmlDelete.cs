using System.Data.XmlClient;

namespace Data.Xml.XmlIO.Delete;

internal class XmlDelete : FileDelete<XmlParameter>
{
    public XmlDelete(FileDeleteQuery<XmlParameter> queryParser, FileConnection<XmlParameter> connection, FileCommand<XmlParameter> command) 
        : base(queryParser, connection, command)
    {
        dataSetWriter = new XmlDataSetWriter(connection, queryParser);
    }
}

