using System.Data.CsvClient;

namespace Data.Csv.CsvIO.Write;

internal class CsvInsert : FileInsert
{
    public CsvInsert(FileInsertQuery queryParser, FileConnection<CsvParameter> jsonConnection, FileCommand<CsvParameter> jsonCommand) 
        : base(queryParser, jsonConnection, jsonCommand)
    {
        dataSetWriter = new CsvDataSetWriter(jsonConnection, queryParser);
    }

    public override bool SchemaUnknownWithoutData => false;

    protected override object DefaultIdentityValue() => "1";

    protected override bool DecimalHandled(DataColumn dataColumn, DataRow lastRow, DataRow newRow)
    {
        var lastRowColumnValue = lastRow[dataColumn.ColumnName].ToString();
        if (decimal.TryParse(lastRowColumnValue, out decimal columnValueAsInt))
        {
            newRow[dataColumn.ColumnName] = unchecked(columnValueAsInt + 1).ToString();
            return true;
        }

        return false;
    }

}
