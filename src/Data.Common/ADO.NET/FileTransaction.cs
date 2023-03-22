namespace System.Data.FileClient;

public class FileTransaction : IDbTransaction
{
    private readonly FileConnection connection;
    private readonly IsolationLevel isolationLevel;
    internal bool TransactionDone { get; private set; } = false;

    public FileTransaction(FileConnection connection, IsolationLevel isolationLevel = default)
    {
        this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
        this.isolationLevel = isolationLevel;
    }

    public readonly List<FileWriter> Writers = new List<FileWriter>();

    public void Commit()
    {
        if (TransactionDone)
        {
            throw new InvalidOperationException("This JsonTransaction has completed; it is no longer usable");
        }
        TransactionDone = true;

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

    public void Rollback()
    {
        //We don't need to do anything as we haven't saved the data to database
        if (TransactionDone)
        {
            throw new InvalidOperationException("This JsonTransaction has completed; it is no longer usable");
        }
        TransactionDone = true;
    }

    public void Dispose()
    {
        Writers.Clear();
    }

    public IDbConnection Connection => connection;

    public IsolationLevel IsolationLevel => isolationLevel;
}