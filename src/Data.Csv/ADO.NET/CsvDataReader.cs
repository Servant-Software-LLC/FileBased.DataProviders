using Data.Common.Utils;

namespace System.Data.CsvClient;

public class CsvDataReader : FileDataReader
{
    internal CsvDataReader(IEnumerable<FileStatement> fileStatements, 
                           FileReader fileReader,
                           TransactionScopedRows transactionScopedRows,
                           Func<FileStatement, FileWriter> createWriter, 
                           LoggerServices loggerServices) 
        : base(fileStatements, fileReader, transactionScopedRows, createWriter, loggerServices)
    {
    }
}   