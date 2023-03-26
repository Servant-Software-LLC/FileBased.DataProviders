namespace Data.Csv.CsvIO.Delete;

internal class CsvDelete : FileDelete
{
    public CsvDelete(FileDeleteQuery queryParser, FileConnection connection, FileCommand jsonCommand) 
        : base(queryParser, connection, jsonCommand)
    {
        this.dataSetWriter = new CsvDataSetWriter(connection,queryParser);
    }
}

