using Data.Common.Utils;

namespace System.Data.CsvClient;

public class CsvDataReader : FileDataReader
{
    internal CsvDataReader(IEnumerable<FileStatement> fileStatements, 
                           FileReader fileReader, 
                           Func<FileStatement, FileWriter> createWriter, 
                           LoggerServices loggerServices) 
        : base(fileStatements, fileReader, createWriter, loggerServices)
    {
    }
}   