using SqlBuildingBlocks.LogicalEntities;

namespace Data.Common.Utils;

/// <summary>
/// A representation of the stored data plus any data that has been added in the current transaction via INSERT statements.
/// </summary>
internal class TransactionLevelData
{
    private readonly DataSet storedData;
    private readonly string databaseName;
    private readonly TransactionScopedRows transactionScopedRows;

    public TransactionLevelData(DataSet storedData, string databaseName, TransactionScopedRows transactionScopedRows)
    {
        this.storedData = storedData ?? throw new ArgumentNullException(nameof(storedData));
        this.databaseName = !string.IsNullOrWhiteSpace(databaseName) ? databaseName : throw new ArgumentNullException(nameof(databaseName));
        this.transactionScopedRows = transactionScopedRows ?? throw new ArgumentNullException(nameof(transactionScopedRows));
    }

    public DataSet Compose(IEnumerable<SqlTable> tablesInvolvedInSqlStatement)
    {
        DataSet database = new(databaseName);
        foreach (SqlTable table in tablesInvolvedInSqlStatement)
        {
            //See if it is in this database (i.e. stored DataSet)?
            if (string.Compare(databaseName, table.DatabaseName) != 0)
            {
                //This case occurs if the request is for a schema metadata.
                continue;
            }

            var storedDataTable = storedData.Tables[table.TableName];
            if (storedDataTable == null)
                throw new InvalidOperationException($"The table {table} does not exist in the database.");


            //Copy the DataTable schema and data.
            //TODO:  This is probably an expensive operation.  Consider more performant approaches.
            DataTable copiedTable = storedDataTable.Copy();

            AddInsertedRowsOfTransaction(table, copiedTable);

            database.Tables.Add(copiedTable);
        }

        return database;
    }

    private void AddInsertedRowsOfTransaction(SqlTable table, DataTable copiedTable)
    {
        //Check to see if there is a pending transaction that is INSERTing new rows.
        if (transactionScopedRows != null && transactionScopedRows.TryGetValue(table.TableName, out List<DataRow> transactionInsertedRows))
        {
            //In this case, we want to add the rows from the transaction into this copied DataTable
            foreach (DataRow additionalRow in transactionInsertedRows)
            {
                var newRow = copiedTable.NewRow();

                // Copy the data.
                for (int i = 0; i < additionalRow.Table.Columns.Count; i++)
                {
                    newRow[i] = additionalRow[i];
                }

                copiedTable.Rows.Add(newRow);
            }
        }
    }

}
