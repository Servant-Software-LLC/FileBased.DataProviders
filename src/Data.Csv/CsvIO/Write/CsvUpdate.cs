using System.Data.CsvClient;

namespace Data.Csv.CsvIO.Write;

internal class CsvUpdate : FileUpdate<CsvParameter>
{
    public CsvUpdate(FileUpdateQuery<CsvParameter> queryParser, FileConnection<CsvParameter> FileConnection, FileCommand<CsvParameter> FileCommand) 
        : base(queryParser, FileConnection, FileCommand)
    {
        dataSetWriter = new CsvDataSetWriter(FileConnection, queryParser);
    }
}
