using System.Data.XmlClient;

namespace Data.Xml.XmlIO.Write;

internal class XmlInsert : Common.FileIO.Write.FileInsertWriter
{
    public XmlInsert(Common.FileStatements.FileInsert fileStatement, FileConnection<XmlParameter> jsonConnection, FileCommand<XmlParameter> jsonCommand) 
        : base(fileStatement, jsonConnection, jsonCommand)
    {
    }

    //TODO:  So far our support is for XML with the xsd definitions.  Later, this value may become a "it depends on whether xsd was included in the file"
    public override bool SchemaUnknownWithoutData => false;

    protected override object DefaultIdentityValue() => 1;

    protected override void RealizeSchema(DataTable dataTable) => throw new NotImplementedException();
}
