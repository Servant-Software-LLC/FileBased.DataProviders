namespace System.Data.JsonClient;

public class JsonDataReader : FileDataReader
{
    internal JsonDataReader(IEnumerable<FileStatement> fileStatements, FileReader FileReader, Func<FileStatement, FileWriter> createWriter) 
        : base(fileStatements, FileReader, createWriter)
    {
    }
}