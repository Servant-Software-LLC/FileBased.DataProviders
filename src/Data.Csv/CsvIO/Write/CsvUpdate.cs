using System.Data.CsvClient;

namespace Data.Csv.CsvIO.Write;

internal class CsvUpdate : Common.FileIO.Write.FileUpdateWriter
{
    public CsvUpdate(Common.FileStatements.FileUpdate fileStatement, FileConnection<CsvParameter> FileConnection, FileCommand<CsvParameter> FileCommand) 
        : base(fileStatement, FileConnection, FileCommand)
    {
        dataSetWriter = new CsvDataSetWriter(FileConnection, fileStatement);
    }
}
