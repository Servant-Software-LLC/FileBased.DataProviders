using Data.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace System.Data.FileClient;

public abstract class FileTransaction<TFileParameter> : DbTransaction, IFileTransaction
    where TFileParameter : FileParameter<TFileParameter>, new()
{
    private ILogger<FileTransaction<TFileParameter>> log => connection.LoggerServices.CreateLogger<FileTransaction<TFileParameter>>();

    private readonly FileConnection<TFileParameter> connection;
    private readonly IsolationLevel isolationLevel;
    public bool TransactionDone { get; private set; } = false;

    public FileTransaction(FileConnection<TFileParameter> connection, IsolationLevel isolationLevel = default)
    {
        this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
        this.isolationLevel = isolationLevel;

        log.LogInformation($"{GetType()} constructed.");
    }

    public List<FileWriter> Writers { get; } = new();

    public override IsolationLevel IsolationLevel => isolationLevel;
    public override void Commit()
    {
        if (TransactionDone)
        {
            throw new InvalidOperationException("This JsonTransaction has completed; it is no longer usable");
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

    public override void Rollback()
    {
        //We don't need to do anything as we haven't saved the data to database
        if (TransactionDone)
        {
            throw new InvalidOperationException("This JsonTransaction has completed; it is no longer usable");
        }
        TransactionDone = true;

        log.LogInformation($"{GetType()}.{nameof(Rollback)}() called.");
    }

    protected new void Dispose()
    {
        log.LogDebug($"{GetType()}.{nameof(Dispose)}() called.");

        base.Dispose();
        Writers.Clear();
    }

    protected override DbConnection DbConnection => connection;
    public abstract FileCommand<TFileParameter> CreateCommand(string cmdText);
}