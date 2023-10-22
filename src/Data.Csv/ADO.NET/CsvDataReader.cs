using Data.Common.Utils;

namespace System.Data.CsvClient;

/// <summary>
/// Represents a data reader for CSV operations.
/// </summary>
public class CsvDataReader : FileDataReader
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CsvDataReader"/> class.
    /// </summary>
    /// <param name="fileStatements">The file statements to be executed.</param>
    /// <param name="fileReader">The file reader to use for reading data.</param>
    /// <param name="transactionScopedRows">The transaction-scoped rows for the data reader.</param>
    /// <param name="createWriter">The function to create a file writer.</param>
    /// <param name="loggerServices">The logger services for logging.</param>
    internal CsvDataReader(IEnumerable<FileStatement> fileStatements, 
                           FileReader fileReader,
                           TransactionScopedRows transactionScopedRows,
                           Func<FileStatement, FileWriter> createWriter, 
                           LoggerServices loggerServices) 
        : base(fileStatements, fileReader, transactionScopedRows, createWriter, loggerServices)
    {
    }
}   