using System.Data.CsvClient;

namespace Data.Csv.CsvIO.Write;

internal class CsvUpdate : Common.FileIO.Write.FileUpdateWriter
{
    public CsvUpdate(FileUpdate fileStatement, FileConnection<CsvParameter> fileConnection, FileCommand<CsvParameter> fileCommand) 
        : base(fileStatement, fileConnection, fileCommand)
    {
    }
}
