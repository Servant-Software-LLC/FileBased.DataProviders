namespace Data.Common.FileIO;

public abstract class FileWriter<TFileParameter>
    where TFileParameter : FileParameter<TFileParameter>, new()
{
    protected readonly FileCommand<TFileParameter> fileCommand;
    protected readonly FileQuery.FileQuery<TFileParameter> fileQuery;
    protected readonly FileReader<TFileParameter> fileReader;
    protected readonly FileTransaction<TFileParameter>? fileTransaction;
    protected IDataSetWriter? dataSetWriter { get; set; }

    public FileWriter(FileConnection<TFileParameter> fileConnection,
                      FileCommand<TFileParameter> fileCommand,
                      FileQuery.FileQuery<TFileParameter> fileQuery)
    {
        this.fileCommand = fileCommand;
        this.fileQuery = fileQuery;
        fileReader = fileConnection.FileReader;
        fileTransaction = fileCommand.Transaction as FileTransaction<TFileParameter>;
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
