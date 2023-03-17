namespace System.Data.FileClient;
public class FileTransaction : IDbTransaction
{
    private FileConnection _connection;
    private IsolationLevel _isolationLevel;
    internal bool TransactionDone = false;
    public readonly List<FileWriter> Writers 
        =new List<FileWriter>();
    public FileTransaction(FileConnection connection, IsolationLevel isolationLevel = default)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        _isolationLevel = isolationLevel;
    }

    public void Commit()
    {
        if (TransactionDone)
        {
            throw new InvalidOperationException("This JsonTransaction has completed; it is no longer usable");
        }
        TransactionDone = true;
        Writers.ForEach(writer =>
        {
            writer.Execute();
        });

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

    public IDbConnection Connection => _connection;

    public IsolationLevel IsolationLevel => _isolationLevel;
}