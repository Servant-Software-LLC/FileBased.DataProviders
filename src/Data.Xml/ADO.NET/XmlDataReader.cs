using Data.Common.Utils;

namespace System.Data.XmlClient;

public class XmlDataReader : FileDataReader
{
    internal XmlDataReader(IEnumerable<FileStatement> fileStatements,
                           FileReader fileReader, 
                           Func<FileStatement, FileWriter> createWriter,
                           LoggerServices loggerServices) 
        : base(fileStatements, fileReader, createWriter, loggerServices)
    {
    }
}