using Data.Common.FileIO.Create;
using System.Data.CsvClient;

namespace Data.Csv.CsvIO.Create;

internal class CsvCreateTable : FileCreateTableWriter
{
    public CsvCreateTable(FileCreateTable fileStatement, FileConnection<CsvParameter> FileConnection, FileCommand<CsvParameter> FileCommand)
        : base(fileStatement, FileConnection, FileCommand)
    {
        dataSetWriter = new CsvDataSetWriter(FileConnection, fileStatement);
    }
}