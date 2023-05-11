using System.Data.CsvClient;

namespace Data.Csv.CsvIO.Write;

internal class CsvInsert : FileInsert<CsvParameter>
{
    public CsvInsert(FileInsertQuery<CsvParameter> queryParser, FileConnection<CsvParameter> jsonConnection, FileCommand<CsvParameter> jsonCommand) 
        : base(queryParser, jsonConnection, jsonCommand)
    {
        dataSetWriter = new CsvDataSetWriter(jsonConnection, queryParser);
    }

    public override bool SchemaUnknownWithoutData => false;
}
