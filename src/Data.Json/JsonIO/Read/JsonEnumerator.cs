namespace Data.Json.JsonIO.Read;
internal class JsonEnumerator : IEnumerator<object?[]>
{
    private readonly DataView workingDataView;  // We cannot use the DefaultView of the DataTable, because workingResultset may be a pointer directly to
                                                // one of the tables (i.e. not created from the columns/joins of the SELECT query) and many JsonDataReader/JsonEnumerator
                                                // may be instantiated with different filters on them.
    private object?[] currentRow = Array.Empty<object>();

    public JsonEnumerator(IEnumerable<string> resultSetColumnNames, DataTable workingResultset, Filter? filter)
    {
        if (workingResultset == null) 
            throw new ArgumentNullException(nameof(workingResultset));

        workingDataView = new DataView(workingResultset);
        if (filter != null)
        {
            workingDataView.RowFilter = filter.Evaluate();
        }

        Columns.AddRange(resultSetColumnNames);
        if (Columns.FirstOrDefault()?.Trim() == "*" && Columns != null)
        {
            Columns.Clear();
            foreach (DataColumn column in workingResultset.Columns)
            {
                Columns.Add(column.ColumnName);
            }
        }
    }

    public object?[] Current => currentRow;
    object IEnumerator.Current => Current;
    public int CurrentIndex { get; private set; } = -1;
    public List<string> Columns { get; } = new List<string>();
    public int FieldCount => Columns.Count;


    public bool MoveNext()
    {
        CurrentIndex++;
        if (workingDataView.Count > CurrentIndex)
        {
            var row = workingDataView[CurrentIndex].Row;
            if (Columns?.FirstOrDefault()?.Trim() != "*")
            {
                currentRow = new object?[Columns!.Count];
                for (int i = 0; i < Columns?.Count; i++)
                {
                    currentRow[i] = row[Columns[i]];
                }
            }
            else
            {
                currentRow = row.ItemArray;
            }
            return true;
        }
        return false;
    }
    public bool MoveNextInitial()
    {
        var res = MoveNext();
        Reset();
        return res;
    }
    public void Reset()
    {
        CurrentIndex = -1;
        //TableEnumerator.Reset();
    }

    public void Dispose()
    {
        //TableEnumerator?.Reset();
    }

    public string GetName(int i) => Columns[i];
    public int GetOrdinal(string name) => Columns.IndexOf(name);
    
    public Type GetType(int i)
    {
        var name = GetName(i);
        return workingDataView.Table!.Columns[name]!.DataType;
    }

}
