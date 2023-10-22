using Data.Common.Utils;

namespace System.Data.JsonClient;

/// <summary>
/// Provides a means of reading one or more forward-only streams of result sets obtained from evaluating a JSON data source.
/// </summary>
public class JsonDataReader : FileDataReader
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JsonDataReader"/> class with the specified parameters.
    /// </summary>
    /// <param name="fileStatements">The enumerable of file statements used to retrieve data.</param>
    /// <param name="fileReader">The file reader to use for reading data from the data source.</param>
    /// <param name="transactionScopedRows">The rows that are in the scope of a transaction.</param>
    /// <param name="createWriter">The function that creates a writer for updating the data source based on a file statement.</param>
    /// <param name="loggerServices">The services used for logging.</param>
    internal JsonDataReader(IEnumerable<FileStatement> fileStatements, 
                            FileReader fileReader,
                            TransactionScopedRows transactionScopedRows,
                            Func<FileStatement, FileWriter> createWriter,
                            LoggerServices loggerServices) 
        : base(fileStatements, fileReader, transactionScopedRows, createWriter, loggerServices)
    {
    }
}