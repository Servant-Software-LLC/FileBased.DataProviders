using System.Data.XmlClient;

namespace Data.Xml.XmlIO.Delete;

internal class XmlDelete : Common.FileIO.Delete.FileDeleteWriter
{
    public XmlDelete(Common.FileStatements.FileDelete fileStatement, FileConnection<XmlParameter> connection, FileCommand<XmlParameter> command) 
        : base(fileStatement, connection, command)
    {
        dataSetWriter = new XmlDataSetWriter(connection, fileStatement);
    }
}

