using System.Data.XmlClient;

namespace Data.Xml.XmlIO.Write;

internal class XmlUpdate : Common.FileIO.Write.FileUpdateWriter
{
    public XmlUpdate(Common.FileStatements.FileUpdate fileStatement, FileConnection<XmlParameter> FileConnection, FileCommand<XmlParameter> FileCommand) 
        : base(fileStatement, FileConnection, FileCommand)
    {
    }
}
