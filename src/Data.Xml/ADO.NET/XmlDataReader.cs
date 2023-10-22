using Data.Common.Utils;

namespace System.Data.XmlClient;

/// <summary>
/// Provides a way of reading a forward-only stream of rows from an XML file.
/// </summary>
public class XmlDataReader : FileDataReader
{
    /// <summary>
    /// Initializes a new instance of the <see cref="XmlDataReader"/> class with the specified parameters.
    /// </summary>
    /// <param name="fileStatements">The file statements to be executed.</param>
    /// <param name="fileReader">The file reader used to read data from the file.</param>
    /// <param name="transactionScopedRows">The transaction-scoped rows.</param>
    /// <param name="createWriter">The function used to create a file writer.</param>
    /// <param name="loggerServices">The logger services used for logging.</param>
    internal XmlDataReader(IEnumerable<FileStatement> fileStatements,
                           FileReader fileReader,
                           TransactionScopedRows transactionScopedRows,
                           Func<FileStatement, FileWriter> createWriter,
                           LoggerServices loggerServices) 
        : base(fileStatements, fileReader, transactionScopedRows, createWriter, loggerServices)
    {
    }
}