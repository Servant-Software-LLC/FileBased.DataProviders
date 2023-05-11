using System.Data.CsvClient;

namespace Data.Csv.CsvIO.Delete;

internal class CsvDelete : FileDelete<CsvParameter>
{
    public CsvDelete(FileDeleteQuery<CsvParameter> queryParser, FileConnection<CsvParameter> connection, FileCommand<CsvParameter> command) 
        : base(queryParser, connection, command)
    {
        this.dataSetWriter = new CsvDataSetWriter(connection, queryParser);
    }
}

