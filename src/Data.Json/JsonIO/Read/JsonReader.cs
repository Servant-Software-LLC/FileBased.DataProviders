using Data.Json.JsonQuery;
using System.Data.JsonClient;

namespace Data.Json.JsonIO.Read
{
    internal class JsonReader : IEnumerator<object?[]>
    {
        object?[] _currentRow;
        public JsonReader(JsonCommand jsonCommand,JsonConnection jsonConnection)
        {
            this.jsonConnection = jsonConnection;
            jsonConnection.JsonReader.JsonQueryParser = jsonCommand.QueryParser;
            jsonConnection.JsonReader.ReadJson();
            var filter = jsonCommand.QueryParser.Filter;
            if (filter!=null)
            {
                jsonConnection.JsonReader.DataSet!.Tables[0].DefaultView.RowFilter = filter.Evaluate();
            }
            columns =new List<string>(jsonCommand.QueryParser.GetColumns());
            if (columns?.FirstOrDefault()?.Trim() == "*"&&columns!=null)
            {
                columns.Clear();
                foreach (DataColumn column in jsonConnection.JsonReader.DataTable.Columns)
                {
                    columns.Add(column.ColumnName);
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
              return  columns.Count;
            }
        }

        public int currentIndex = -1;
        private readonly JsonConnection jsonConnection;
        private readonly List<string> columns;

        public bool MoveNext()
        {
            currentIndex++;
            if (jsonConnection.JsonReader.DataTable.DefaultView.Count > currentIndex)
            {
                var row = jsonConnection.JsonReader.DataTable.DefaultView[currentIndex].Row;
                if (columns?.FirstOrDefault()?.Trim() != "*")
                {
                    _currentRow = new object?[columns.Count];
                    for (int i = 0; i < columns?.Count; i++)
                    {
                        _currentRow[i] = row[columns[i]];
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
           return columns[i];
        }
        internal int GetOrdinal(string name)
        {
            return columns.IndexOf(name);
        }
        internal Type GetType(int i)
        {
            var name = GetName(i);
            return jsonConnection.JsonReader.DataTable.Columns[name]!.DataType;
        }

    }
}
