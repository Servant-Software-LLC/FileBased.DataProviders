using Data.Common.FileIO.Create;
using System.Data.XmlClient;

namespace Data.Xml.XmlIO.Create;

internal class XmlCreateTable : FileCreateTableWriter
{
    public XmlCreateTable(FileCreateTable fileStatement, FileConnection<XmlParameter> FileConnection, FileCommand<XmlParameter> FileCommand)
        : base(fileStatement, FileConnection, FileCommand)
    {
        dataSetWriter = new XmlDataSetWriter(FileConnection, fileStatement);
    }
}