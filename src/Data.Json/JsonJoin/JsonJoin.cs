namespace Data.Json.JsonJoin
{
    public struct JoinFilter
    {
        public string Table1Name { get; set; }
        public string Table1Col { get; set; }
        public string Table2Col { get; set; }
        public string Op { get; set; }
    }
        public class JsonJoin
    {
        public IEnumerable<string> Tables { get; set; }
        =Enumerable.Empty<string>();
        IList<JoinFilter> JoinFilters { get; set; }
        public JsonJoin()
        {

        }
        public IEnumerable<JsonTable> GetTables()
        {
            return JoinFilters?.Select(filter => new JsonTable(filter.Table1Name));

        }
        public bool Join(DataRow mainRow,DataSet dataSet)
        {
            if (JoinFilters==null)
            {
                return true;
            }
            bool canjoin = false;
            foreach (var filter in JoinFilters)
            {
                //var table2TempName = filter.Table1Col.Split('.')[0];
                ////var table2Name = GetTableNameFromTemp(table2TempName,tables);
                ////Get Table 1 Name
                //var table1 = dataSet.Tablestables.First(x=>x.tableName==filter.Table1Name);
                //var table2 = tables.First(x => x.tableName.EndsWith(table2TempName));
                //var val1 = table2.element.GetValue(filter.Table1Col.Split('.')[1]);
                //var simpleFilter = new SimpleFilter(filter.Table2Col.Split('.')[1], filter.Op, val1!);
                //canjoin = simpleFilter.Evaluate(table1.element);
                //if (!canjoin)
                //{
                //    return false;
                //}
            }
            return canjoin;
        }

        private static (string tableName, JsonElement element) GetTable(IEnumerable<(string tableName, JsonElement element)> tables, string tableName)
        {
            return tables.First(x => x.tableName.Split(' ')[0].Trim() == tableName);
        }

        private string GetTableNameFromTemp(string table2TempName, IEnumerable<(string tableName, JsonElement element)> tables)
        {
            var table = tables.First(x => x.tableName.Split(' ').Last().Trim() == table2TempName);
            return table.tableName.Split(' ')[0].Trim();
        }


        public void AddTable(JoinFilter joinFilter)
        {
            JoinFilters ??= new List<JoinFilter>();
            JoinFilters.Add(joinFilter);
        }
    }
}
