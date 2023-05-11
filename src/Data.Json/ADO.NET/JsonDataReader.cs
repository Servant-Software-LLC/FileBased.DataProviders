namespace System.Data.JsonClient;

public class JsonDataReader : FileDataReader<JsonParameter>
{
    internal JsonDataReader(FileQuery<JsonParameter> queryParser, FileReader<JsonParameter> FileReader) 
        : base(queryParser, FileReader)
    {
    }
}