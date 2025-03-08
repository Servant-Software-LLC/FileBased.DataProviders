using Microsoft.Extensions.Logging;
using SqlBuildingBlocks.POCOs;

namespace Data.Common.FileIO.Write;

public abstract class FileInsertWriter : FileWriter
{
    private ILogger<FileInsertWriter> log => fileConnection.LoggerServices.CreateLogger<FileInsertWriter>();

    public FileInsertWriter(FileInsert fileStatement, IFileConnection fileConnection, IFileCommand fileCommand)
        : base(fileConnection, fileCommand, fileStatement)
    {
    }

    /// <summary>
    /// Can columns be added to an empty table (data-wise) with an INSERT?  
    /// </summary>
    public abstract bool SchemaUnknownWithoutData { get; }

    /// <summary>
    /// Identity value generated during the execution of this INSERT statement.
    /// </summary>
    public object LastInsertIdentity { get; private set; }

    public (string TableName, DataRow Row)? TransactionScopedRow { get; private set; }

    public override int Execute()
    {
        log.LogDebug($"{nameof(FileInsertWriter)}.{nameof(Execute)}() called.  IsTransactedLater = {IsTransactedLater}");

        if (fileStatement is not FileInsert fileInsertStatement)
            throw new Exception($"Expected {nameof(fileStatement)} to be a {nameof(FileInsert)}");

        if (IsTransactedLater)
        {
            fileTransaction!.Writers.Add(this);

            //Call PrepareRow() in order to determine the identity value
            var results = PrepareRow(fileInsertStatement);
            TransactionScopedRow = (results.Table.TableName, results.Row);

            return 1;
        }
        try
        {
            if (!IsTransaction)
            {
                readerWriterLock.EnterWriteLock();
                //as we have modified the json file so we don't need to update the tables
                fileReader.StopWatching();

                var results = PrepareRow(fileInsertStatement);
                results.Table.AppendRow(results.Row);
            }
            else
            {
                var transactionScopedRow = TransactionScopedRow.Value;
                var table = fileReader.DataSet.Tables[transactionScopedRow.TableName];

                table.AppendRow(transactionScopedRow.Row);
            }

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
        return 1;
    }

    private (VirtualDataTable Table, DataRow Row) PrepareRow(FileInsert fileStatement)
    {
        var virtualDataTable = fileReader.ReadFile(fileStatement, fileTransaction?.TransactionScopedRows);

        //Check if we need to add columns on the first INSERT of data into this table.
        if (SchemaUnknownWithoutData && !virtualDataTable.Rows.Any())
        {
            //Remember here, that the whole data table is going to reside in-memory at this point.
            var dataTable = virtualDataTable.ToDataTable();

            RealizeSchema(dataTable);

            //Schema may have changed the columns and rows.
            virtualDataTable.Columns = dataTable.Columns;
            virtualDataTable.Rows = virtualDataTable.Rows;
        }

        var rowValues = fileStatement.GetValues();

        //Ensure that the row values use DBNull instead of null.
        foreach (KeyValuePair<string, object> keyValuePair in rowValues)
        {
            rowValues[keyValuePair.Key] = keyValuePair.Value ?? DBNull.Value;
        }

        AddMissingValues(rowValues, virtualDataTable);

        var row = virtualDataTable.CreateNewRowFromData(rowValues);
        return (virtualDataTable, row);
    }

    protected virtual object DefaultIdentityValue() => Convert.ChangeType(1, fileConnection.PreferredFloatingPointDataType.ToType());
    protected virtual bool GuidHandled(DataColumn dataColumn, DataRow lastRow, IDictionary<string, object> newRow)
    {
        var lastRowColumnValue = lastRow[dataColumn.ColumnName].ToString();
        if (Guid.TryParse(lastRowColumnValue, out Guid columnValueAsGuid))
        {
            newRow[dataColumn.ColumnName] = Guid.NewGuid().ToString();
            return true;
        }

        return false;
    }

    protected virtual bool FloatingPointHandled(DataColumn dataColumn, DataRow lastRow, IDictionary<string, object> newRow)
    {
        if (dataColumn.DataType == typeof(float) || dataColumn.DataType == typeof(double) || dataColumn.DataType == typeof(decimal))
        {
            var lastRowColumnValue = lastRow[dataColumn.ColumnName];
            if (lastRowColumnValue is float floatValue)
                newRow[dataColumn.ColumnName] = floatValue + 1f;
            else if (lastRowColumnValue is double doubleValue)
                newRow[dataColumn.ColumnName] = doubleValue + 1d;
            else if (lastRowColumnValue is decimal decValue)
                newRow[dataColumn.ColumnName] = decValue + 1m;

            return true;
        }

        return false;
    }

    private void AddMissingValues(IDictionary<string, object> newRow, VirtualDataTable virtualDataTable)
    {
        // Creating this DataTable may consume a lot of memory.  Making this Lazy to avoid unless it is necessary
        //Lazy<DataTable> dataTable = new Lazy<DataTable>(() => virtualDataTable.ToDataTable());

        foreach (DataColumn dataColumn in virtualDataTable.Columns)
        {
            var columnName = dataColumn.ColumnName;
            var identityColumn = ColumnNameIndicatesIdentity(columnName);
            if (!identityColumn)
            {
                // If the column value is missing, then add its default value
                if (!newRow.TryGetValue(columnName, out object rowValue))
                {
                    //Add it with the default value.
                    newRow[columnName] = dataColumn.DefaultValue;

                    continue;
                }

                // Make sure that the new value is like the DataColumn's DataType
                if (rowValue != DBNull.Value && rowValue.GetType() != dataColumn.DataType)
                {
                    newRow[columnName] = Convert.ChangeType(rowValue, dataColumn.DataType);
                }
            }

            //Assuming that columns that don't have their default value are columns that have been set
            //and so they don't need to check if they are identity columns
            if (newRow.ContainsKey(columnName) && !object.Equals(newRow[columnName], dataColumn.DefaultValue))
                continue;

            // Look for missing identity values
            if (ColumnNameIndicatesIdentity(columnName))
            {
                // This could be an expensive operation depending on number of rows here.
                var lastRow = virtualDataTable.Rows.Cast<DataRow>().LastOrDefault();

                //Since we don't have a datatype for values in a CSV, we need to determine if the last
                //row 'looks' like a datatype that can be an identity (i.e. Guid or integer).

                bool handled = false;
                try
                {

                    //If there isn't a lastRow, then assume the datatype is integer and start from 1.                
                    if (lastRow == null)
                    {
                        newRow[columnName] = DefaultIdentityValue();
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
                    if (FloatingPointHandled(dataColumn, lastRow, newRow))
                    {
                        handled = true;
                        continue;
                    }
                        
                    //The lastRow value of the column isn't recognized, so we don't want to guess.
                }
                finally
                {
                    if (handled)
                        LastInsertIdentity = newRow[columnName];
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
