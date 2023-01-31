using Data.Json.JsonQuery;

namespace Data.Json.JsonIO.Read
{
    internal abstract class JsonDataAccess
    {
        public DataSet DataSet = new DataSet();
        protected readonly JsonDocument jsonDocument;
        protected readonly JsonQueryParser queryParser;

        protected JsonDataAccess(JsonDocument jsonDocument, JsonQueryParser queryParser)
        {
            ArgumentNullException.ThrowIfNull(jsonDocument, nameof(jsonDocument));
            ArgumentNullException.ThrowIfNull(queryParser, nameof(queryParser));
            this.jsonDocument = jsonDocument;
            this.queryParser = queryParser;
            Prepare(jsonDocument.RootElement);
            if (queryParser.Filter != null)
                DataSet.Tables[queryParser.Table].DefaultView.RowFilter = queryParser.Filter.ToString();
        }

        public virtual IEnumerable<string> GetTables()
        {

            var li = new List<string>() { queryParser.Table };
            return li;
        }
        public void Prepare(JsonElement rootElement)
        {
            if (queryParser)
            {
                var tables = GetTables();
                //enumerate via tables
                var database = rootElement.EnumerateObject();
                //get the userdefined table in the query
                foreach (var item in database.Where(x => tables.Any(y => y== x.Name)))
                {
                    //create datatable
                    var dataTable = new DataTable(item.Name);
                    //add to dataset
                    DataSet.Tables.Add(dataTable);
                    foreach (var col in GetFields(item.Value))
                    {
                        var datatype = item.Value.ValueKind.GetClrFieldType();
                        dataTable.Columns.Add(col.name, col.type);
                    }
                    //fill datatables
                    foreach (var row in item.Value.EnumerateArray())
                    {
                        var newRow = dataTable.NewRow();
                        foreach (var field in row.EnumerateObject())
                        {
                            var val = field.Value.GetValue();
                            if(val!=null)
                            newRow[field.Name] = val;
                        }
                        dataTable.Rows.Add(newRow);
                    }
                }
            }
            foreach (DataTable item in DataSet.Tables)
            {
                FieldCount += item.Columns.Count;
            }
        }

        public int FieldCount { get; private set; }
        public IEnumerable<(string name, Type type)> GetFields(JsonElement table)
        {
            var maxFieldElement = table.EnumerateArray().MaxBy(x =>
             {
                 return x.EnumerateObject().Count();
             });
            var enumerator = maxFieldElement.EnumerateObject();
            return enumerator.Select(x => (x.Name, x.Value.ValueKind.GetClrFieldType()));
        }
    }
}
