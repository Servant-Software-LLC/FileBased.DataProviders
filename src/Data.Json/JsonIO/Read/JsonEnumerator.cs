namespace Data.Json.JsonIO.Read;

internal class JsonEnumerator : IEnumerator<object?[]>
{
    private object?[] _currentRow=new object[0];

    public JsonEnumerator(JsonCommand jsonCommand, JsonConnection jsonConnection)
    {
        if (jsonCommand == null)
            throw new ArgumentNullException(nameof(jsonCommand));

        this.jsonConnection = jsonConnection ?? throw new ArgumentNullException(nameof(jsonConnection));
        jsonConnection.JsonReader.JsonQueryParser = jsonCommand.QueryParser;
        jsonConnection.JsonReader.ReadJson(true);
        var filter = jsonCommand.QueryParser!.Filter;
        if (filter!=null)
        {
            var tableName = jsonCommand.QueryParser.Table;
            jsonConnection.JsonReader.DataSet!.Tables[tableName]!.DefaultView.RowFilter = filter.Evaluate();
        }
        Columns = new List<string>(jsonCommand.QueryParser.GetColumns());
        if (Columns.FirstOrDefault()?.Trim() == "*" && Columns != null)
        {
            Columns.Clear();
            foreach (DataColumn column in jsonConnection.JsonReader.DataTable.Columns)
            {
                Columns.Add(column.ColumnName);
            }
        }
    }
    public object?[] Current
    {
        get
        {
            return _currentRow;
        }
    }
    object IEnumerator.Current => Current;


    public int FieldCount
    {
        get
        {
          return  Columns.Count;
        }
    }

    public int currentIndex = -1;
    private readonly JsonConnection jsonConnection;
    internal readonly List<string> Columns = new List<string>();

    public bool MoveNext()
    {
        currentIndex++;
        if (jsonConnection.JsonReader.DataTable.DefaultView.Count > currentIndex)
        {
            var row = jsonConnection.JsonReader.DataTable.DefaultView[currentIndex].Row;
            if (Columns?.FirstOrDefault()?.Trim() != "*")
            {
                _currentRow = new object?[Columns!.Count];
                for (int i = 0; i < Columns?.Count; i++)
                {
                    _currentRow[i] = row[Columns[i]];
                }
            }
            else
            {
                _currentRow = row.ItemArray;
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
        currentIndex = -1;
        //TableEnumerator.Reset();
    }

    public void Dispose()
    {
        jsonConnection.JsonReader.Dispose();
        //TableEnumerator?.Reset();
    }
   internal string GetName(int i)
    {
       return Columns[i];
    }
    internal int GetOrdinal(string name)
    {
        return Columns.IndexOf(name);
    }
    internal Type GetType(int i)
    {
        var name = GetName(i);
        return jsonConnection.JsonReader.DataTable.Columns[name]!.DataType;
    }

}
