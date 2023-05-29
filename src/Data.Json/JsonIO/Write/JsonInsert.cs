using Data.Common.FileIO.Write;
using System.Data.JsonClient;

namespace Data.Json.JsonIO.Write;

internal class JsonInsert : FileInsert
{
    public JsonInsert(FileInsertQuery queryParser, FileConnection<JsonParameter> jsonConnection, FileCommand<JsonParameter> jsonCommand) 
        : base(queryParser, jsonConnection, jsonCommand)
    {
        dataSetWriter = new JsonDataSetWriter(jsonConnection, queryParser);
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
