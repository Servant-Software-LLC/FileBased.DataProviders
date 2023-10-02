using Data.Common.FileIO.Create;
using System.Data.CsvClient;

namespace Data.Csv.CsvIO.Create;

internal class CsvCreateTable : FileCreateTableWriter
{
    public CsvCreateTable(FileCreateTable fileStatement, FileConnection<CsvParameter> fileConnection, FileCommand<CsvParameter> fileCommand)
        : base(fileStatement, fileConnection, fileCommand)
    {
    }
}