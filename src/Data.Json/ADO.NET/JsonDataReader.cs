using Data.Common.Utils;

namespace System.Data.JsonClient;

public class JsonDataReader : FileDataReader
{
    internal JsonDataReader(IEnumerable<FileStatement> fileStatements, 
                            FileReader FileReader, 
                            Func<FileStatement, FileWriter> createWriter,
                            LoggerServices loggerServices) 
        : base(fileStatements, FileReader, createWriter, loggerServices)
    {
    }
}