using System.Data.XmlClient;

namespace Data.Xml.XmlIO.Write;

internal class XmlInsert : Common.FileIO.Write.FileInsertWriter
{
    public XmlInsert(Common.FileStatements.FileInsert fileStatement, FileConnection<XmlParameter> jsonConnection, FileCommand<XmlParameter> jsonCommand)
        : base(fileStatement, jsonConnection, jsonCommand)
    {
    }

    public override bool SchemaUnknownWithoutData => true;

    protected override void RealizeSchema(DataTable dataTable)
    {
        // If schema is already defined (e.g. from an XSD file or CREATE TABLE), nothing to do.
        if (dataTable.Columns.Count > 0)
            return;

        if (fileStatement is not FileInsert fileInsertStatement)
            throw new Exception($"Expected {nameof(fileStatement)} to be a {nameof(FileInsert)}");

        // Infer schema from the insert values when no XSD schema is available.
        // Unlike JSON which re-infers types on read, XML persists column types in XSD,
        // so numeric types must use the preferred floating point type for consistency.
        var preferredNumericType = fileConnection.PreferredFloatingPointDataType.ToType();
        foreach (var val in fileInsertStatement.GetValues())
        {
            var dataColumnType = GetDataColumnType(val.Value);
            if (dataColumnType == typeof(decimal))
                dataColumnType = preferredNumericType;
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
