namespace Data.Common.FileIO.Delete;

public abstract class FileDelete<TFileParameter> : FileWriter<TFileParameter>
    where TFileParameter : FileParameter<TFileParameter>, new()
{
    private readonly FileDeleteQuery<TFileParameter> query;

    public FileDelete(FileDeleteQuery<TFileParameter> queryParser, FileConnection<TFileParameter> fileConnection, FileCommand<TFileParameter> fileCommand)
        : base(fileConnection, fileCommand, queryParser)
    {
        query = queryParser ?? throw new ArgumentNullException(nameof(queryParser));
    }

    public override int Execute()
    {
        try
        {
            if (!IsTransaction)
            {
                //as we have modified the json file so we don't need to update the tables
                _rwLock.EnterWriteLock();
                fileReader.StopWatching();
            }

            var dataTable = fileReader.ReadFile(query);

            //Create a DataView to work with just for this operation
            var dataView = new DataView(dataTable);
            dataView.RowFilter = query.Filter?.ToString();

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
          
            return rowsAffected;
        }
        finally
        {
            Save();
            if (!IsTransaction)
            {
                fileReader.StartWatching();
                _rwLock.ExitWriteLock();
            }
        }
    }
}

