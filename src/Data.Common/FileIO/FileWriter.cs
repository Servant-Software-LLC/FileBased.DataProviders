namespace Data.Common.FileIO;

public abstract class FileWriter
{
    protected readonly IFileCommand fileCommand;
    protected readonly FileStatement fileStatement;
    protected readonly IFileConnection fileConnection;
    protected readonly FileReader fileReader;
    protected readonly IFileTransaction? fileTransaction;
    private readonly Lazy<IDataSetWriter> dataSetWriter;

    public FileWriter(IFileConnection fileConnection,
                      IFileCommand fileCommand,
                      FileStatement fileStatement)
    {
        this.fileCommand = fileCommand;
        this.fileStatement = fileStatement;
        this.fileConnection = fileConnection; 
        fileReader = fileConnection.FileReader;
        fileTransaction = fileCommand.FileTransaction;

        dataSetWriter = new(((IFileConnectionInternal)fileConnection).CreateDataSetWriter(fileStatement));
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

        dataSetWriter.Value.WriteDataSet(fileReader.DataSet!);
      
        return true;
    }
  
}
