using Microsoft.Extensions.Logging;

namespace System.Data.CsvClient;

/// <summary>
/// Represents a database transaction to be performed on a CSV file.
/// </summary>
/// <remarks>
/// This class is used to manage a sequence of operations that can be committed or rolled back as a single unit. It is derived from <see cref="FileTransaction&lt;TCsvParameter&gt;"/> class.
/// </remarks>
public class CsvTransaction : FileTransaction<CsvParameter>
{
    private ILogger<CsvTransaction> log => ((IFileConnection)connection).LoggerServices.CreateLogger<CsvTransaction>();
    private readonly CsvConnection connection;

    /// <summary>
    /// Initializes a new instance of the <see cref="CsvTransaction"/> class.
    /// </summary>
    /// <param name="connection">The <see cref="CsvConnection"/> to be used for this transaction.</param>
    /// <param name="isolationLevel">The isolation level of the transaction. Default is <see cref="IsolationLevel.Unspecified"/>.</param>
    public CsvTransaction(CsvConnection connection, IsolationLevel isolationLevel = 0) 
        : base(connection, isolationLevel)
    {
        this.connection = connection;
    }

    /// <summary>
    /// Creates a new <see cref="CsvCommand"/> object associated with the transaction.
    /// </summary>
    /// <param name="cmdText">The command text for the <see cref="CsvCommand"/>.</param>
    /// <returns>A new instance of <see cref="CsvCommand"/>.</returns>
    public override CsvCommand CreateCommand(string cmdText)
    {
        log.LogDebug($"{GetType()}.{nameof(CreateCommand)}() called.  CommandText = {cmdText}");
        return new(cmdText, connection, this);
    }
}