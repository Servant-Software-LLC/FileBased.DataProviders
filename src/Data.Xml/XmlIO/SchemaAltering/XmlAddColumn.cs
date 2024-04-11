using Data.Common.FileIO.SchemaAltering;
using System.Data.XmlClient;

namespace Data.Xml.XmlIO.SchemaAltering;

internal class XmlAddColumn : FileAddColumnWriter
{
    public XmlAddColumn(FileAddColumn fileStatement, FileConnection<XmlParameter> fileConnection, FileCommand<XmlParameter> fileCommand)
        : base(fileStatement, fileConnection, fileCommand)
    {
    }
}