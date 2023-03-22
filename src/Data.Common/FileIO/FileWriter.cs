namespace Data.Common.FileIO;

public abstract class FileWriter
{
    protected readonly FileCommand jsonCommand;
    protected readonly FileQuery.FileQuery jsonQuery;
    protected readonly FileReader fileReader;
    protected readonly FileTransaction? fileTransaction;
    protected IDataSetWriter? dataSetWriter { get; set; }

    public FileWriter(FileConnection jsonConnection,
                      FileCommand jsonCommand,
                      FileQuery.FileQuery jsonQuery)
    {
        this.jsonCommand = jsonCommand;
        this.jsonQuery = jsonQuery;
        fileReader = jsonConnection.FileReader;
        fileTransaction = jsonCommand.Transaction as FileTransaction;
    }

    public abstract int Execute();    
    internal static ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim();

    public bool IsTransaction
        => fileTransaction != null;
    public bool IsTransactedLater
        => fileTransaction != null && fileTransaction?.TransactionDone == false;

    public virtual bool Save()
    {
        if (IsTransactedLater)
        {
            return true;
        }
        dataSetWriter!.WriteDataSet(fileReader.DataSet!);
      
        return true;
    }
  
}
