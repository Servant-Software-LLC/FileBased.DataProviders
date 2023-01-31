using Data.Json.JsonQuery;

namespace Data.Json.JsonIO.Write
{
    internal class JsonInsert : JsonWriter
    {
        public JsonInsert(JsonCommand command, JsonDocument jsonDocument, JsonInsertQuery query) : base(command, jsonDocument, query)
        {
            Query = query;
            this.columnValues = query.GetValues();
        }
        public JsonInsertQuery Query { get; }
        private readonly IEnumerable<KeyValuePair<string, object>> columnValues;
        public override int Execute()
        {
            DataTable datatable = DataSet.Tables[queryParser.Table]!;
            var row = datatable.NewRow();
            foreach (var val in columnValues)
            {
                row[val.Key] = val.Value;
            }
            datatable.Rows.Add(row);
            Save();
            return 1;
        }
    }
}
