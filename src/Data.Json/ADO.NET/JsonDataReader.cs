namespace System.Data.JsonClient;

public class JsonDataReader : FileDataReader
{
    internal JsonDataReader(IEnumerable<FileQuery> queryParsers, FileReader FileReader) 
        : base(queryParsers, FileReader)
    {
    }
}