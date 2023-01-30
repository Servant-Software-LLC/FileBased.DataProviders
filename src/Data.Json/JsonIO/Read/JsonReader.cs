namespace Data.Json.JsonIO.Read
{
    internal class JsonReader : JsonDataAccess, IEnumerator<DataRow>
    {
        DataRow? _currentRow;
        public JsonReader(JsonQueryParser jSONQuery, JsonDocument jsonDocument) : base(jsonDocument, jSONQuery)
        {
           
        }
        public DataRow Current
        {
            get
            {
                return _currentRow!;
            }
        }
        object IEnumerator.Current => Current;
        public int currentIndex = -1;
        public bool MoveNext()
        {
            currentIndex++;
            if (DataSet.Tables[0].DefaultView.Count > currentIndex)
            {
                _currentRow = DataSet.Tables[0].DefaultView[currentIndex].Row;
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
            currentIndex = 0;
            //TableEnumerator.Reset();
        }

        public void Dispose()
        {
            DataSet.Dispose();
            //TableEnumerator?.Reset();
        }
       internal string GetName(int i)
        {
           return DataSet.Tables[0].Columns[i].ColumnName;
        }
        internal int GetOrdinal(string name)
        {
            return DataSet.Tables[0].Columns[name].Ordinal;
        }
        internal Type GetType(int i)
        {
            return DataSet.Tables[0].Columns[i].DataType;
        }

    }
}
