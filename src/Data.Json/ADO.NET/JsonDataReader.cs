using Data.Common.Utils;

namespace System.Data.JsonClient;

public class JsonDataReader : FileDataReader
{
    internal JsonDataReader(IEnumerable<FileStatement> fileStatements, 
                            FileReader fileReader,
                            TransactionScopedRows transactionScopedRows,
                            Func<FileStatement, FileWriter> createWriter,
                            LoggerServices loggerServices) 
        : base(fileStatements, fileReader, transactionScopedRows, createWriter, loggerServices)
    {
    }
}