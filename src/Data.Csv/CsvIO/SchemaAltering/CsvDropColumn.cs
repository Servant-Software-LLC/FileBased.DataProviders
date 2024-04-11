using Data.Common.FileIO.SchemaAltering;
using System.Data.CsvClient;

namespace Data.Csv.CsvIO.SchemaAltering;

internal class CsvDropColumn : FileDropColumnWriter
{
    public CsvDropColumn(FileDropColumn fileStatement, FileConnection<CsvParameter> fileConnection, FileCommand<CsvParameter> fileCommand)
        : base(fileStatement, fileConnection, fileCommand)
    {
    }
}