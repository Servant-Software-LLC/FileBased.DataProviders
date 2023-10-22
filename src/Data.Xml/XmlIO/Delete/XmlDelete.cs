using System.Data.XmlClient;

namespace Data.Xml.XmlIO.Delete;

internal class XmlDelete : Common.FileIO.Delete.FileDeleteWriter
{
    public XmlDelete(Common.FileStatements.FileDelete fileStatement, FileConnection<XmlParameter> fileConnection, FileCommand<XmlParameter> fileCommand) 
        : base(fileStatement, fileConnection, fileCommand)
    {
    }
}

