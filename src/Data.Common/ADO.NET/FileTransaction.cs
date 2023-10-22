using Data.Common.Utils;
using Microsoft.Extensions.Logging;

namespace System.Data.FileClient;


/// <summary>
/// Represents a transaction to be performed at a data store, with support for commit and rollback operations.
/// </summary>
/// <typeparam name="TFileParameter">The type of file parameter.</typeparam>
public abstract class FileTransaction<TFileParameter> : DbTransaction, IFileTransaction
    where TFileParameter : FileParameter<TFileParameter>, new()
{
    private ILogger<FileTransaction<TFileParameter>> log => connection.LoggerServices.CreateLogger<FileTransaction<TFileParameter>>();

    private readonly FileConnection<TFileParameter> connection;
    private readonly IsolationLevel isolationLevel;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileTransaction{TFileParameter}"/> class.
    /// </summary>
    /// <param name="connection">The file connection.</param>
    /// <param name="isolationLevel">The isolation level.</param>
    public FileTransaction(FileConnection<TFileParameter> connection, IsolationLevel isolationLevel = default)
    {
        this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
        this.isolationLevel = isolationLevel;

        log.LogInformation($"{GetType()} constructed.");
    }

    /// <summary>
    /// Gets a value indicating whether the transaction is done.
    /// </summary>
    public bool TransactionDone { get; private set; } = false;

    /// <summary>
    /// Gets the list of file writers.
    /// </summary>
    public List<FileWriter> Writers { get; } = new();

    /// <summary>
    /// Gets the transaction scoped rows.
    /// </summary>
    public TransactionScopedRows TransactionScopedRows { get; } = new();


    /// <inheritdoc/>
    public override IsolationLevel IsolationLevel => isolationLevel;

    /// <inheritdoc/>
    public override void Commit()
    {
        if (TransactionDone)
        {
            throw new InvalidOperationException($"This {GetType().Name} has completed; it is no longer usable");
        }
        TransactionDone = true;

        log.LogInformation($"{GetType()}.{nameof(Commit)}() called.");

        FileWriter._rwLock.EnterWriteLock();
        try
        {
            Writers.ForEach(writer =>
            {
                writer.Execute();
            });
        }
        finally
        {
            FileWriter._rwLock.ExitWriteLock();
        }

    }

    /// <inheritdoc/>
    public override void Rollback()
    {
        //We don't need to do anything as we haven't saved the data to database
        if (TransactionDone)
        {
            throw new InvalidOperationException($"This {GetType().Name} has completed; it is no longer usable");
        }
        TransactionDone = true;

        log.LogInformation($"{GetType()}.{nameof(Rollback)}() called.");
    }

    /// <inheritdoc/>
    protected new void Dispose()
    {
        log.LogDebug($"{GetType()}.{nameof(Dispose)}() called.");

        base.Dispose();
        Writers.Clear();
    }

    /// <inheritdoc/>
    protected override DbConnection DbConnection => connection;

    /// <summary>
    /// Creates a file command.
    /// </summary>
    /// <param name="cmdText">The command text.</param>
    /// <returns>The file command.</returns>
    public abstract FileCommand<TFileParameter> CreateCommand(string cmdText);
}
