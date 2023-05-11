namespace Data.Common.FileIO.Write;

public abstract class FileInsert<TFileParameter> : FileWriter<TFileParameter>
    where TFileParameter : FileParameter<TFileParameter>, new()
{
    private readonly FileInsertQuery<TFileParameter> queryParser;

    public FileInsert(FileInsertQuery<TFileParameter> queryParser, FileConnection<TFileParameter> fileConnection, FileCommand<TFileParameter> fileCommand)
        : base(fileConnection, fileCommand,(FileQuery.FileQuery<TFileParameter>)queryParser)
    {
        this.queryParser = queryParser ?? throw new ArgumentNullException(nameof(queryParser));
    }

    /// <summary>
    /// Can columns be added to an empty table (data-wise) with an INSERT?  
    /// </summary>
    public abstract bool SchemaUnknownWithoutData { get; }

    public override int Execute()
    {
        if (IsTransactedLater)
        {
            fileTransaction!.Writers.Add(this);
            return 1;
        }
        try
        {
            if (!IsTransaction)
            {
                _rwLock.EnterWriteLock();
                //as we have modified the json file so we don't need to update the tables
                fileReader.StopWatching();
            }

            var dataTable = fileReader.ReadFile(queryParser);
            
            //Check if we need to add columns on the first INSERT of data into this table.
            if (SchemaUnknownWithoutData && dataTable.Columns.Count == 0)
            {
                AddMissingColumns(dataTable);
            }

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

            if (!IsTransaction)
            {
                fileReader.StartWatching();
                _rwLock.ExitWriteLock();
            }
        }
        return 1;
    }

    private void AddMissingColumns(DataTable dataTable)
    {
        foreach (var val in queryParser.GetValues())
        {
            var jsonType = GetJsonType(val.Value);
            dataTable.Columns.Add(val.Key, jsonType);
        }
    }

    private Type GetJsonType(object value) => value switch
    {
        int intValue => typeof(decimal),
        long longValue => typeof(decimal),
        decimal decimalValue => typeof(decimal),
        string stringValue => typeof(string),
        bool boolValue => typeof(bool),
        null => typeof(string),
        _ => throw new InvalidOperationException("query not supported")
    };
}
