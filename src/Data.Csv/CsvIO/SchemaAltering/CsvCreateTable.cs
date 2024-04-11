using Data.Common.FileIO.SchemaAltering;
using System.Data.CsvClient;

namespace Data.Csv.CsvIO.SchemaAltering;

internal class CsvCreateTable : FileCreateTableWriter
{
    public CsvCreateTable(FileCreateTable fileStatement, FileConnection<CsvParameter> fileConnection, FileCommand<CsvParameter> fileCommand)
        : base(fileStatement, fileConnection, fileCommand)
    {
    }
}