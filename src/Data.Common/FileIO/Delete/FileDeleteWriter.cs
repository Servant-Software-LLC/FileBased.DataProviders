namespace Data.Common.FileIO.Delete;

public abstract class FileDeleteWriter : FileWriter
{
    private readonly FileDelete query;

    public FileDeleteWriter(FileDelete fileStatement, IFileConnection fileConnection, IFileCommand fileCommand)
        : base(fileConnection, fileCommand, fileStatement)
    {
        query = fileStatement ?? throw new ArgumentNullException(nameof(fileStatement));
    }

    public override int Execute()
    {
        try
        {
            if (!IsTransaction)
            {
                //as we have modified the database file so we don't need to update the tables
                readerWriterLock.EnterWriteLock();
                fileReader.StopWatching();
            }

            var virtualDataTable = fileReader.ReadFile(query, fileTransaction?.TransactionScopedRows);

            //Remember here, that the whole data table is going to reside in-memory at this point.
            var dataTable = virtualDataTable.ToDataTable();

            //Create a DataView to work with just for this operation
            var dataView = new DataView(dataTable);
            dataView.RowFilter = query.Filter?.ToExpressionString();

            var rowsAffected = dataView.Count;

            //don't update now if it is a transaction
            if (IsTransactedLater)
            {
                fileTransaction!.Writers.Add(this);
                return rowsAffected;
            }

            foreach (DataRowView dataRow in dataView)
            {
                dataTable!.Rows.Remove(dataRow.Row);
            }
            
            //Save the results of the deletion back onto the virtual table, which will get saved in the finally with a Save() call below.
            virtualDataTable.Columns = dataTable.Columns;
            virtualDataTable.Rows = dataTable.Rows.Cast<DataRow>();

            return rowsAffected;
        }
        finally
        {
            Save();
            if (!IsTransaction)
            {
                fileReader.StartWatching();
                readerWriterLock.ExitWriteLock();
            }
        }
    }
}

