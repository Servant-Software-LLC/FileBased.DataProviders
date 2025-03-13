using SqlBuildingBlocks.LogicalEntities;
using SqlBuildingBlocks.POCOs;
using System.Data;

namespace Data.Common.Utils;

/// <summary>
/// A representation of the stored data plus any data that has been added in the current transaction via INSERT statements.
/// </summary>
internal class TransactionLevelData
{
    private readonly VirtualDataSet storedData;
    private readonly string databaseName;
    private readonly TransactionScopedRows transactionScopedRows;

    public TransactionLevelData(VirtualDataSet storedData, string databaseName, TransactionScopedRows transactionScopedRows)
    {
        this.storedData = storedData ?? throw new ArgumentNullException(nameof(storedData));
        this.databaseName = !string.IsNullOrWhiteSpace(databaseName) ? databaseName : throw new ArgumentNullException(nameof(databaseName));
        this.transactionScopedRows = transactionScopedRows;
    }

    public VirtualDataSet Compose(IEnumerable<SqlTable> tablesInvolvedInSqlStatement)
    {
        VirtualDataSet database = new();
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

            VirtualDataTable tableWithPotentialTransactionRows = new(table.TableName);
            tableWithPotentialTransactionRows.AdoptDataTable(storedDataTable);
            tableWithPotentialTransactionRows.Rows = AddInsertedRowsOfTransaction(table, tableWithPotentialTransactionRows, storedDataTable.Rows);
           
            database.Tables.Add(tableWithPotentialTransactionRows);
        }

        return database;
    }

    private IEnumerable<DataRow> AddInsertedRowsOfTransaction(SqlTable table, VirtualDataTable tableWithPotentialTransactionRows, IEnumerable<DataRow> sourceRows)
    {
        //Check to see if there is a pending transaction that is INSERTing new rows.
        if (transactionScopedRows != null && transactionScopedRows.TryGetValue(table.TableName, out List<DataRow> transactionInsertedRows))
        {
            transactionInsertedRows = transactionInsertedRows.Select(tableWithPotentialTransactionRows.MatchSchema).ToList();

            //In this case, we want to add the rows from the transaction into this data table.
            return sourceRows.Concat(transactionInsertedRows);
        }

        return sourceRows;
    }

}
