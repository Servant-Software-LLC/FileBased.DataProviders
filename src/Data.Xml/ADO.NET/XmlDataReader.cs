namespace System.Data.XmlClient;

public class XmlDataReader : FileDataReader
{
    internal XmlDataReader(IEnumerable<FileStatement> fileStatements, FileReader fileReader, Func<FileStatement, FileWriter> createWriter) 
        : base(fileStatements, fileReader, createWriter)
    {
    }
}