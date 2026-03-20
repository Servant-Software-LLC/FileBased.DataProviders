using Data.Common.FileIO.SchemaAltering;
using System.Data.CsvClient;

namespace Data.Csv.CsvIO.SchemaAltering;

internal class CsvDropTable : FileDropTableWriter
{
    public CsvDropTable(FileDropTable fileStatement, FileConnection<CsvParameter> fileConnection, FileCommand<CsvParameter> fileCommand)
        : base(fileStatement, fileConnection, fileCommand)
    {
    }
}
