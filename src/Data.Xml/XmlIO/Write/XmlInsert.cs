namespace Data.Xml.XmlIO.Write;

internal class XmlInsert : FileInsert
{
    public XmlInsert(FileInsertQuery queryParser, FileConnection jsonConnection, FileCommand jsonCommand) 
        : base(queryParser, jsonConnection, jsonCommand)
    {
        dataSetWriter = new XmlDataSetWriter(jsonConnection, queryParser);
    }

    //TODO:  So far our support is for XML with the xsd definitions.  Later, this value may become a "it depends on whether xsd was included in the file"
    public override bool SchemaUnknownWithoutData => false;
}
