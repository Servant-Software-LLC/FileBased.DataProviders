namespace System.Data.CsvClient;

public class CsvDataReader : FileDataReader<CsvParameter>
{
    internal CsvDataReader(FileQuery<CsvParameter> queryParser, FileReader<CsvParameter> fileReader) 
        : base(queryParser, fileReader)
    {
    }
}