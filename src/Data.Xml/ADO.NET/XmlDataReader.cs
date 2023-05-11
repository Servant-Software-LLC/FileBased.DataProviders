namespace System.Data.XmlClient;

public class XmlDataReader : FileDataReader<XmlParameter>
{
    internal XmlDataReader(FileQuery<XmlParameter> queryParser, FileReader<XmlParameter> fileReader) 
        : base(queryParser, fileReader)
    {
    }
}