namespace System.Data.XmlClient;

public class CsvDataReader : FileDataReader
{
    public CsvDataReader(FileQuery queryParser, FileReader fileReader) 
        : base(queryParser, fileReader)
    {
    }
}