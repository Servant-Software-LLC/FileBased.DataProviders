using Microsoft.Extensions.Logging;

namespace Data.Common.FileIO.Write;

public abstract class FileInsertWriter : FileWriter
{
    private ILogger<FileInsertWriter> log => fileConnection.LoggerServices.CreateLogger<FileInsertWriter>();
    protected readonly FileInsert fileStatement;

    public FileInsertWriter(FileInsert fileStatement, IFileConnection fileConnection, IFileCommand fileCommand)
        : base(fileConnection, fileCommand, fileStatement)
    {
        this.fileStatement = fileStatement ?? throw new ArgumentNullException(nameof(fileStatement));
    }

    /// <summary>
    /// Can columns be added to an empty table (data-wise) with an INSERT?  
    /// </summary>
    public abstract bool SchemaUnknownWithoutData { get; }

    /// <summary>
    /// Identity value generated during the execution of this INSERT statement.
    /// </summary>
    public object? LastInsertIdentity { get; private set; }

    public (string TableName, DataRow Row)? TransactionScopedRow { get; private set; }

    public override int Execute()
    {
        log.LogDebug($"{nameof(FileInsertWriter)}.{nameof(Execute)}() called.  IsTransactedLater = {IsTransactedLater}");

        if (IsTransactedLater)
        {
            fileTransaction!.Writers.Add(this);

            //Call PrepareRow() in order to determine the identity value
            var results = PrepareRow(fileStatement);
            TransactionScopedRow = (results.Table.TableName, results.Row);
            return 1;
        }
        try
        {
            if (!IsTransaction)
            {
                _rwLock.EnterWriteLock();
                //as we have modified the json file so we don't need to update the tables
                fileReader.StopWatching();

                var results = PrepareRow(fileStatement);
                results.Table.Rows.Add(results.Row);
            }
            else
            {
                var transactionScopedRow = TransactionScopedRow.Value;
                var table = fileReader.DataSet.Tables[transactionScopedRow.TableName];
                table.Rows.Add(transactionScopedRow.Row.ItemArray);
            }

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

    private (DataTable Table, DataRow Row) PrepareRow(FileInsert fileStatement)
    {
        var dataTable = fileReader.ReadFile(fileStatement, fileTransaction?.TransactionScopedRows);

        //Check if we need to add columns on the first INSERT of data into this table.
        if (SchemaUnknownWithoutData && dataTable.Rows.Count == 0)
        {
            RealizeSchema(dataTable);
        }

        var row = dataTable!.NewRow();
        foreach (var val in fileStatement.GetValues())
        {
            row[val.Key] = val.Value ?? DBNull.Value;
        }

        AddMissingIdentityValues(dataTable, row);
        return (dataTable, row);
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

    public static bool Default_DecimalHandled(DataColumn dataColumn, DataRow lastRow, DataRow newRow)
    {
        if (dataColumn.DataType == typeof(decimal))
        {
            var lastRowColumnValue = lastRow[dataColumn.ColumnName];
            if (lastRowColumnValue is decimal decValue)
                newRow[dataColumn.ColumnName] = decValue + 1m;

            return true;
        }

        return false;
    }

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

                bool handled = false;
                try
                {

                    //If there isn't a lastRow, then assume the datatype is integer and start from 1.                
                    if (lastRow == null)
                    {
                        newRow[dataColumn.ColumnName] = DefaultIdentityValue();
                        handled = true;
                        continue;
                    }

                    //Does the value of this column in the last row look like a Guid?
                    if (GuidHandled(dataColumn, lastRow, newRow))
                    {
                        handled = true;
                        continue;
                    }


                    //Does the value of this column in the last row look like an decimal or is one?
                    if (DecimalHandled(dataColumn, lastRow, newRow))
                    {
                        handled = true;
                        continue;
                    }
                        
                    //The lastRow value of the column isn't recognized, so we don't want to guess.
                }
                finally
                {
                    if (handled)
                        LastInsertIdentity = newRow[dataColumn.ColumnName];
                }
            }

        }
    }


    protected static bool ColumnNameIndicatesIdentity(string columnName) =>
        string.Compare(columnName, "Id", true) == 0 || columnName.EndsWith("Id", StringComparison.InvariantCultureIgnoreCase);

    protected abstract void RealizeSchema(DataTable dataTable);

    protected Type GetDataColumnType(object value) => value switch
    {
        int => typeof(decimal),
        long => typeof(decimal),
        decimal => typeof(decimal),
        string => typeof(string),
        bool => typeof(bool),
        null => typeof(string),
        _ => throw new InvalidOperationException("query not supported")
    };
}
