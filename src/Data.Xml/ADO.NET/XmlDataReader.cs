namespace System.Data.XmlClient;

public class XmlDataReader : FileDataReader
{
    internal XmlDataReader(IEnumerable<FileQuery> queryParsers, FileReader fileReader) 
        : base(queryParsers, fileReader)
    {
    }
}