using Data.Common.FileIO.Create;
using System.Data.XmlClient;

namespace Data.Xml.XmlIO.Create;

internal class XmlCreateTable : FileCreateTableWriter
{
    public XmlCreateTable(FileCreateTable fileStatement, FileConnection<XmlParameter> fileConnection, FileCommand<XmlParameter> fileCommand)
        : base(fileStatement, fileConnection, fileCommand)
    {
    }
}