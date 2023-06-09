namespace Data.Common.FileJoin;

public class DataTableJoin
{
    private readonly IEnumerable<Join> dataTableInnerJoins;
    private readonly string mainTable;

    public DataTableJoin(IEnumerable<Join> dataTableInnerJoins, string mainTable)
    {
        this.dataTableInnerJoins = dataTableInnerJoins;
        this.mainTable = mainTable;
    }

    public DataTable Join(DataSet database, Dictionary<string, List<DataRow>> transactionScopedRows)
    {
        var resultTable = new DataTable();
        foreach (DataTable table in database.Tables)
        {
            foreach (DataColumn col in table.Columns)
            {
                if (resultTable.Columns[col.ColumnName] != null)
                {
                    continue;
                }
                resultTable.Columns.Add(col.ColumnName, col.DataType);
            }
        }

        var mainDataTable = database.Tables[mainTable]!;

        //If we have additional rows to add because we're in a transaction
        if (transactionScopedRows != null && transactionScopedRows.TryGetValue(mainTable, out List<DataRow> additionalRows))
        {
            mainDataTable = mainDataTable.Copy();

            foreach (DataRow additionalRow in additionalRows)
                mainDataTable.Rows.Add(additionalRow);
        }

        foreach (DataRow sourceRow in mainDataTable.Rows)
        {
            var rows = new List<DataRow>();
            foreach (var dataTableInnerJoin in dataTableInnerJoins)
            {
                JoinRows(sourceRow,
                         resultTable,
                         dataTableInnerJoin,
                         database,
                         rows,
                         transactionScopedRows);
            }
            foreach (var item in rows)
            {
                resultTable.Rows.Add(item);
            }
        }
        return resultTable;
    }

    public List<DataRow> JoinRows(DataRow sourceRow,
                                  DataTable resultTable,
                                  Join dataTableInnerJoin,
                                  DataSet database,
                                  List<DataRow> dataRows,
                                  Dictionary<string, List<DataRow>> transactionScopedRows)
    {
        var sourceColumnVal = sourceRow[dataTableInnerJoin.SourceColumn];
        var dataTableToJoin = database.Tables[dataTableInnerJoin.TableName];

        //If we have additional rows to add because we're in a transaction
        if (transactionScopedRows != null && transactionScopedRows.TryGetValue(mainTable, out List<DataRow> additionalRows))
        {
            dataTableToJoin = dataTableToJoin.Copy();

            foreach (DataRow additionalRow in additionalRows)
                dataTableToJoin.Rows.Add(additionalRow);
        }

        var filter = new SimpleFilter(new Field(dataTableInnerJoin.JoinColumn), dataTableInnerJoin.Operation, sourceColumnVal);
        //File.WriteAllText("foo.txt", "foo");
        var dataView = new DataView(dataTableToJoin!);
        var sFilter = filter.Evaluate();
        dataView.RowFilter = sFilter;

        foreach (DataRowView row in dataView)
        {
            var resultRow = resultTable.NewRow();
            if (dataTableInnerJoin.InnerJoin.Count > 0)
            {
                foreach (var innerJoin in dataTableInnerJoin.InnerJoin)
                {
                    var rows = new List<DataRow>();
                    var otherRows = JoinRows(row.Row,
                                            resultTable,
                                            innerJoin,
                                            database,
                                            rows,
                                            transactionScopedRows);

                    foreach (var item in otherRows)
                    {
                        AddRow(item, sourceRow.Table, sourceRow);
                        AddRow(item, dataTableToJoin!, row.Row);
                    }
                    dataRows.AddRange(rows);
                }
                continue;
            }
            AddRow(resultRow, sourceRow.Table, sourceRow);
            AddRow(resultRow, dataTableToJoin!, row.Row);
            dataRows.Add(resultRow);
        }

        return dataRows;
    }

    private static void AddRow(DataRow sourceRow, DataTable dataTableToJoin, DataRow row)
    {
        foreach (DataColumn col in dataTableToJoin.Columns)
        {
            sourceRow[col.ColumnName] = row[col.ColumnName];
        }
    }

}
