namespace System.Data.CsvClient;

public class CsvDataReader : FileDataReader
{
    internal CsvDataReader(IEnumerable<FileStatement> fileStatements, FileReader fileReader, Func<FileStatement, FileWriter> createWriter) 
        : base(fileStatements, fileReader, createWriter)
    {
    }
}   