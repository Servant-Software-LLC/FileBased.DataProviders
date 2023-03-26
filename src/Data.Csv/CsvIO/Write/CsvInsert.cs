namespace Data.Csv.CsvIO.Write;

internal class CsvInsert : FileInsert
{
    public CsvInsert(FileInsertQuery queryParser, FileConnection jsonConnection, FileCommand jsonCommand) 
        : base(queryParser, jsonConnection, jsonCommand)
    {
        dataSetWriter = new CsvDataSetWriter(jsonConnection, queryParser);
    }
}
