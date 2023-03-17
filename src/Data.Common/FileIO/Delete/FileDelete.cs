namespace Data.Common.FileIO.Delete;

public abstract class FileDelete : FileWriter
{
    internal readonly FileDeleteQuery Query;

    public FileDelete(FileDeleteQuery queryParser, FileConnection jsonConnection,FileCommand jsonCommand)
        : base(jsonConnection, jsonCommand, queryParser)
    {
        this.Query = queryParser ?? throw new ArgumentNullException(nameof(queryParser));
    }

    public override int Execute()
    {
        try
        {
            //as we have modified the json file so we don't need to update the tables
            _rwLock.EnterWriteLock();
            fileReader.StopWatching();

            var dataTable = fileReader.ReadFile(Query);

            //Create a DataView to work with just for this operation
            var dataView = new DataView(dataTable);
            dataView.RowFilter = Query.Filter?.ToString();

            var rowsAffected = dataView.Count;
            //don't update now if it is a transaction
            if (base.IsTransaction)
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
            fileReader.StartWatching();
            _rwLock.ExitWriteLock();
        }
    }
}

