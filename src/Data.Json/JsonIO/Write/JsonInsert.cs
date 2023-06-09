using System.Data.JsonClient;

namespace Data.Json.JsonIO.Write;

internal class JsonInsert : Common.FileIO.Write.FileInsertWriter
{
    public JsonInsert(Common.FileStatements.FileInsert fileStatement, FileConnection<JsonParameter> jsonConnection, FileCommand<JsonParameter> jsonCommand) 
        : base(fileStatement, jsonConnection, jsonCommand)
    {
        dataSetWriter = new JsonDataSetWriter(jsonConnection, fileStatement);
    }

    public override bool SchemaUnknownWithoutData => true;

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
