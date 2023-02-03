using Data.Json.JsonQuery;
using System.Data.JsonClient;

namespace Data.Json.JsonIO.Read
{
    internal class JsonReader : IEnumerator<DataRow>
    {
        DataRow? _currentRow;
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
        }
        public DataRow Current
        {
            get
            {
                return _currentRow!;
            }
        }
        object IEnumerator.Current => Current;


        public int FieldCount => jsonConnection.JsonReader.FieldCount;

        public int currentIndex = -1;
        private readonly JsonConnection jsonConnection;

        public bool MoveNext()
        {
            currentIndex++;
            if (jsonConnection.JsonReader.DataSet!.Tables[0].DefaultView.Count > currentIndex)
            {
                _currentRow = jsonConnection.JsonReader.DataSet.Tables[0].DefaultView[currentIndex].Row;
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
           return jsonConnection.JsonReader.DataSet.Tables[0].Columns[i].ColumnName;
        }
        internal int GetOrdinal(string name)
        {
            return jsonConnection.JsonReader.DataSet.Tables[0].Columns[name].Ordinal;
        }
        internal Type GetType(int i)
        {
            return jsonConnection.JsonReader.DataSet.Tables[0].Columns[i].DataType;
        }

    }
}
