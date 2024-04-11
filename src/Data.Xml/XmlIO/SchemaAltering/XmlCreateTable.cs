using Data.Common.FileIO.SchemaAltering;
using System.Data.XmlClient;

namespace Data.Xml.XmlIO.SchemaAltering;

internal class XmlCreateTable : FileCreateTableWriter
{
    public XmlCreateTable(FileCreateTable fileStatement, FileConnection<XmlParameter> fileConnection, FileCommand<XmlParameter> fileCommand)
        : base(fileStatement, fileConnection, fileCommand)
    {
    }
}