using System.Data.CsvClient;

namespace Data.Csv.CsvIO.Delete;

internal class CsvDelete : Common.FileIO.Delete.FileDeleteWriter
{
    public CsvDelete(FileDelete fileStatement, FileConnection<CsvParameter> connection, FileCommand<CsvParameter> command) 
        : base(fileStatement, connection, command)
    {
        dataSetWriter = new CsvDataSetWriter(connection, fileStatement);
    }
}

