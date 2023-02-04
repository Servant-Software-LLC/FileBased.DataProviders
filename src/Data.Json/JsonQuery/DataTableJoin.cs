public class DataTableJoin
{

    public DataTableJoin(string tableName, string column, string @operator, object value)
    {
        Filter = new SimpleFilter(column, @operator, value);

        TableName = tableName;
    }

    public SimpleFilter Filter { get; }
    public string TableName { get; }

    //    public void Join(DataRow sourceRow,DataSet database,DataTable joinTable)
    //    {
    //        var table = database.Tables[TableName];
    //        if (table==null)
    //        {
    //            ThrowHelper.ThrowTableNotFoundException(TableName);
    //            return;
    //        }
    //        table.DefaultView.RowFilter = Filter.Evaluate();


    //        foreach (var item in table.DefaultView)
    //        {
    //            joinTable
    //        }

    //    }
    //}
}

public class RecursiveTableJoin
{
    private readonly Dictionary<string, List<JsonJoin>> _joins;

    public RecursiveTableJoin(Dictionary<string, List<JsonJoin>> joins)
    {
        _joins = joins;
    }

    public DataTable Join(DataSet database, string startTableName)
    {
        var resultTable = new DataTable();
        foreach (DataRow sourceRow in database.Tables[startTableName].Rows)
        {
            resultTable.ImportRow(Join(database, sourceRow, startTableName));
        }
        return resultTable;
    }

    private DataRow Join(DataSet database, DataRow sourceRow, string tableName)
    {
        var joinTable = database.Tables[tableName].Clone();
        joinTable.ImportRow(sourceRow);
        if (!_joins.ContainsKey(tableName))
        {
            return joinTable.Rows[0];
        }
        foreach (var join in _joins[tableName])
        {
            var joinResult = Join(database, sourceRow, join.TableName);
            if (joinResult == null)
            {
                continue;
            }
            joinTable.Rows.Add(joinResult);
        }
        return joinTable.Rows[0];
    }

    public class JsonJoin
    {
        public JsonJoin(string tableName, string joinColumn, string sourceColumn)
        {
            TableName = tableName;
            JoinColumn = joinColumn;
            SourceColumn = sourceColumn;
        }

        public string TableName { get; }
        public string JoinColumn { get; }
        public string SourceColumn { get; }
    }
}
