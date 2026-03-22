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

    protected override void RealizeSchema(DataTable dataTable)
    {
        // If schema is already defined (e.g. from an XSD file or CREATE TABLE), nothing to do.
        if (dataTable.Columns.Count > 0)
            return;

        if (fileStatement is not FileInsert fileInsertStatement)
            throw new Exception($"Expected {nameof(fileStatement)} to be a {nameof(FileInsert)}");

        // Infer schema from the insert values when no XSD schema is available.
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
