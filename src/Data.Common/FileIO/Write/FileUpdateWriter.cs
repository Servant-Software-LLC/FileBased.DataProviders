namespace Data.Common.FileIO.Write;
public class FileUpdateWriter : FileWriter
{
    private readonly FileStatements.FileUpdate fileStatement;

    public FileUpdateWriter(FileStatements.FileUpdate fileStatement, IFileConnection FileConnection, IFileCommand FileCommand) 
        : base(FileConnection, FileCommand, fileStatement)
    {
        this.fileStatement = fileStatement ?? throw new ArgumentNullException(nameof(fileStatement));
    }

    public override int Execute()
    {
        try
        {
            if (!IsTransaction)
            {
                // As we have modified the File file so we don't need to update the tables
                _rwLock.EnterWriteLock();
                fileReader.StopWatching();
            }

            var dataTable = fileReader.ReadFile(fileStatement, fileTransaction?.TransactionScopedRows);
            var values = fileStatement.GetValues();

            //Create a DataView to work with just for this operation
            var dataView = new DataView(dataTable);
            dataView.RowFilter = fileStatement.Filter?.Evaluate();

            var rowsAffected = dataView.Count;

            //don't update now if it is a transaction
            if (IsTransactedLater)
            {
                fileTransaction!.Writers.Add(this);
                return rowsAffected;
            }
            foreach (DataRowView dataRow in dataView)
            {
                foreach (var val in values)
                {
                    dataTable.Columns[val.Key]!.ReadOnly = false;
                    dataRow[val.Key] = val.Value;
                }
            }

            return rowsAffected;
        }
        finally
        {
            Save();

            if (!IsTransaction)
            {
                _rwLock.ExitWriteLock();
                fileReader.StartWatching();
            }
        }

    }
}
