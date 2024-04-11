using Data.Common.FileIO.SchemaAltering;
using System.Data.CsvClient;

namespace Data.Csv.CsvIO.SchemaAltering;

internal class CsvAddColumn : FileAddColumnWriter
{
    public CsvAddColumn(FileAddColumn fileStatement, FileConnection<CsvParameter> fileConnection, FileCommand<CsvParameter> fileCommand)
        : base(fileStatement, fileConnection, fileCommand)
    {
    }
}