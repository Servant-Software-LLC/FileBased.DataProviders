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

    protected override void RealizeSchema(DataTable dataTable)
    {
        //Check to see if the schema has already been defined (this happens in the case that a CREATE TABLE was executed for a JsonCommand)
        if (dataTable.Columns.Count > 0)
            return;

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
                dataTable.Columns.Add(columnNameHint, fileConnection.PreferredFloatingPointDataType.ToType());
            else
                dataTable.Columns.Add(columnNameHint);
        }
    }

}
