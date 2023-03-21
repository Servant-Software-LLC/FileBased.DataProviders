namespace Data.Common.FileIO.Write;

public abstract class FileInsert : FileWriter
{
    private readonly FileInsertQuery queryParser;

    public FileInsert(FileInsertQuery queryParser, FileConnection jsonConnection, FileCommand jsonCommand)
        : base(jsonConnection, jsonCommand,(FileQuery.FileQuery)queryParser)
    {
        this.queryParser = queryParser ?? throw new ArgumentNullException(nameof(queryParser));
    }

    public override int Execute()
    {
        if (IsTransaction)
        {
            fileTransaction!.Writers.Add(this);
            return 1;
        }
        try
        {
            _rwLock.EnterWriteLock();
            //as we have modified the json file so we don't need to update the tables
            fileReader.StopWatching();
            var dataTable = fileReader.ReadFile(queryParser);
            var row = dataTable!.NewRow();
            foreach (var val in queryParser.GetValues())
            {
                row[val.Key] = val.Value;
            }
            
            dataTable.Rows.Add(row);
        }
        finally
        {
            Save();
            fileReader.StartWatching();
            _rwLock.ExitWriteLock();
        }
        return 1;
    }

 
}
