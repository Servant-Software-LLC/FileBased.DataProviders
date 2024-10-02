using Data.Common.FileIO.Write;
using System.Data.JsonClient;

namespace Data.Json.JsonIO.Write;

internal class JsonInsert : FileInsertWriter
{
    public JsonInsert(FileInsert fileStatement, FileConnection<JsonParameter> jsonConnection, FileCommand<JsonParameter> jsonCommand) 
        : base(fileStatement, jsonConnection, jsonCommand)
    {
    }

    public override bool SchemaUnknownWithoutData => true;

    protected override object DefaultIdentityValue() => 1;

    protected override bool DecimalHandled(DataColumn dataColumn, DataRow lastRow, DataRow newRow) => 
        Default_DecimalHandled(dataColumn, lastRow, newRow);

    protected override void RealizeSchema(DataTable dataTable)
    {
        //Add missing columns, since we can now determine the schema

        foreach (var val in fileStatement.GetValues())
        {
            var dataColumnType = GetDataColumnType(val.Value);
            dataTable.Columns.Add(val.Key, dataColumnType);
        }

        foreach (var columnNameHint in fileStatement.ColumnNameHints)
        {
            if (dataTable.Columns.Contains(columnNameHint))
                continue;

            if (ColumnNameIndicatesIdentity(columnNameHint))
                dataTable.Columns.Add(columnNameHint, typeof(decimal));
            else
                dataTable.Columns.Add(columnNameHint);
        }
    }

}
