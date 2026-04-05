using Data.Common.FileIO.SchemaAltering;
using System.Data.XmlClient;

namespace Data.Xml.XmlIO.SchemaAltering;

internal class XmlDropTable : FileDropTableWriter
{
    public XmlDropTable(FileDropTable fileStatement, FileConnection<XmlParameter> fileConnection, FileCommand<XmlParameter> fileCommand)
        : base(fileStatement, fileConnection, fileCommand)
    {
    }
}
