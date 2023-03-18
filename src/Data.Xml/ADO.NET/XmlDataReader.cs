namespace System.Data.XmlClient;

public class XmlDataReader : FileDataReader
{
    public XmlDataReader(FileQuery queryParser, 
        FileReader fileReader) 
        : base(queryParser, fileReader)
    {
    }
}