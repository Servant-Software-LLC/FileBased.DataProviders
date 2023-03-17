namespace Data.Common.FileIO.Write;
public class FileUpdate : FileWriter
{
    private readonly FileUpdateQuery queryParser;

    public FileUpdate(FileUpdateQuery queryParser, FileConnection FileConnection, FileCommand FileCommand) 
        : base(FileConnection, FileCommand, queryParser)
    {
        this.queryParser = queryParser ?? throw new ArgumentNullException(nameof(queryParser));
    }

    public override int Execute()
    {
        try
        {
            // As we have modified the File file so we don't need to update the tables
            _rwLock.EnterWriteLock();
            fileReader.StopWatching();

            var dataTable = fileReader.ReadFile(queryParser);
            var values = queryParser.GetValues();

            //Create a DataView to work with just for this operation
            var dataView = new DataView(dataTable);
            dataView.RowFilter = queryParser.Filter?.Evaluate();

            var rowsAffected = dataView.Count;
            //don't update now if it is a transaction
            if (base.IsTransaction)
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
            _rwLock.ExitWriteLock();
            fileReader.StartWatching();
        }

    }
}
