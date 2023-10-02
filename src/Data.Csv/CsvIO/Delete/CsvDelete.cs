using System.Data.CsvClient;

namespace Data.Csv.CsvIO.Delete;

internal class CsvDelete : Common.FileIO.Delete.FileDeleteWriter
{
    public CsvDelete(FileDelete fileStatement, FileConnection<CsvParameter> fileConnection, FileCommand<CsvParameter> fileCommand) 
        : base(fileStatement, fileConnection, fileCommand)
    {
    }
}

