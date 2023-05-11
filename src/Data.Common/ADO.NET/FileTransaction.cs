namespace System.Data.FileClient;

public abstract class FileTransaction<TFileParameter> : DbTransaction
    where TFileParameter : FileParameter<TFileParameter>, new()
{
    private readonly FileConnection<TFileParameter> connection;
    private readonly IsolationLevel isolationLevel;
    internal bool TransactionDone { get; private set; } = false;

    public FileTransaction(FileConnection<TFileParameter> connection, IsolationLevel isolationLevel = default)
    {
        this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
        this.isolationLevel = isolationLevel;
    }

    public readonly List<FileWriter<TFileParameter>> Writers = new();

    public override IsolationLevel IsolationLevel => isolationLevel;
    public override void Commit()
    {
        if (TransactionDone)
        {
            throw new InvalidOperationException("This JsonTransaction has completed; it is no longer usable");
        }
        TransactionDone = true;

        FileWriter<TFileParameter>._rwLock.EnterWriteLock();
        try
        {
            Writers.ForEach(writer =>
            {
                writer.Execute();
            });
        }
        finally
        {
            FileWriter<TFileParameter>._rwLock.ExitWriteLock();
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
    }

    protected new void Dispose()
    {
        base.Dispose();
        Writers.Clear();
    }

    protected override DbConnection DbConnection => connection;
    public abstract FileCommand<TFileParameter> CreateCommand(string cmdText);
}