namespace System.Data.CsvClient;

public class CsvDataReader : FileDataReader
{
    internal CsvDataReader(IEnumerable<FileQuery> queryParsers, FileReader fileReader) 
        : base(queryParsers, fileReader)
    {
    }
}   