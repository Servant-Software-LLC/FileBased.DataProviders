namespace System.Data.JsonClient;
public class JsonDataReader : FileDataReader
{
    public JsonDataReader(FileQuery queryParser, 
        FileReader FileReader) 
        : base(queryParser, FileReader)
    {
    }
}