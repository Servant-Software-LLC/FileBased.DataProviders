using System.Data.XmlClient;

namespace Data.Xml.XmlIO.Write;

internal class XmlInsert : FileInsert<XmlParameter>
{
    public XmlInsert(FileInsertQuery<XmlParameter> queryParser, FileConnection<XmlParameter> jsonConnection, FileCommand<XmlParameter> jsonCommand) 
        : base(queryParser, jsonConnection, jsonCommand)
    {
        dataSetWriter = new XmlDataSetWriter(jsonConnection, queryParser);
    }

    //TODO:  So far our support is for XML with the xsd definitions.  Later, this value may become a "it depends on whether xsd was included in the file"
    public override bool SchemaUnknownWithoutData => false;

    protected override object DefaultIdentityValue() => 1;

    protected override bool DecimalHandled(DataColumn dataColumn, DataRow lastRow, DataRow newRow)
    {
        if (dataColumn.DataType == typeof(decimal))
        {
            var lastRowColumnValue = lastRow[dataColumn.ColumnName];
            if (lastRowColumnValue is decimal decValue)
                newRow[dataColumn.ColumnName] = decValue + 1m;

            return true;
        }

        return false;
    }
}
