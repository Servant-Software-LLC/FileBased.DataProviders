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

        if (fileStatement is not FileInsert fileInsertStatement)
            throw new Exception($"Expected {nameof(fileStatement)} to be a {nameof(FileInsert)}");

        //Add missing columns, since we can now determine the schema

        foreach (var val in fileInsertStatement.GetValues())
        {
            var dataColumnType = GetDataColumnType(val.Value);
            dataTable.Columns.Add(val.Key, dataColumnType);
        }

        foreach (var columnNameHint in fileInsertStatement.ColumnNameHints)
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
