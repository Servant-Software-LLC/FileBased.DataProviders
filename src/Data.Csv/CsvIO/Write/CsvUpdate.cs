using System.Data.CsvClient;

namespace Data.Csv.CsvIO.Write;

internal class CsvUpdate : FileUpdate
{
    public CsvUpdate(FileUpdateQuery queryParser, FileConnection<CsvParameter> FileConnection, FileCommand<CsvParameter> FileCommand) 
        : base(queryParser, FileConnection, FileCommand)
    {
        dataSetWriter = new CsvDataSetWriter(FileConnection, queryParser);
    }
}
