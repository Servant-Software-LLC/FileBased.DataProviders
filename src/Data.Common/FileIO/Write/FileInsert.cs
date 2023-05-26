using System.Data.Common;

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

            AddMissingIdentityValues(dataTable, row);
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

    protected abstract object DefaultIdentityValue();
    protected virtual bool GuidHandled(DataColumn dataColumn, DataRow lastRow, DataRow newRow)
    {
        var lastRowColumnValue = lastRow[dataColumn.ColumnName].ToString();
        if (Guid.TryParse(lastRowColumnValue, out Guid columnValueAsGuid))
        {
            newRow[dataColumn.ColumnName] = Guid.NewGuid().ToString();
            return true;
        }

        return false;
    }

    protected abstract bool DecimalHandled(DataColumn dataColumn, DataRow lastRow, DataRow newRow);

    private void AddMissingIdentityValues(DataTable dataTable, DataRow newRow)
    {
        foreach (DataColumn dataColumn in dataTable.Columns)
        {
            //Assuming that columns that don't have their default value are columns that have been set
            //and so they don't need to check if they are identity columns
            if (!object.Equals(newRow[dataColumn], dataColumn.DefaultValue))
                continue;

            if (ColumnNameIndicatesIdentity(dataColumn.ColumnName))
            {
                var lastRow = dataTable.Rows.Cast<DataRow>().LastOrDefault();

                //Since we don't have a datatype for values in a CSV, we need to determine if the last
                //row 'looks' like a datatype that can be an identity (i.e. Guid or integer).

                //If there isn't a lastRow, then assume the datatype is integer and start from 1.                
                if (lastRow == null)
                {
                    newRow[dataColumn.ColumnName] = DefaultIdentityValue();
                    continue;
                }

                //Does the value of this column in the last row look like a Guid?
                if (GuidHandled(dataColumn, lastRow, newRow))
                    continue;


                //Does the value of this column in the last row look like an decimal or is one?
                if (DecimalHandled(dataColumn, lastRow, newRow))
                    continue;

                //The lastRow value of the column isn't recognized, so we don't want to guess.
            }

        }
    }


    protected static bool ColumnNameIndicatesIdentity(string columnName) =>
        string.Compare(columnName, "Id", true) == 0 || columnName.EndsWith("Id", StringComparison.InvariantCultureIgnoreCase);

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
