using Data.Json.JsonQuery;

namespace Data.Json.JsonIO.Write
{
    internal class JsonInsert : JsonWriter
    {
        public JsonInsert(JsonCommand command,JsonConnection jsonConnection)
            : base(command, jsonConnection)
        {
            Query = (JsonInsertQuery)command.QueryParser;
        }
        public JsonInsertQuery Query { get; }
        public override int Execute()
        {
            JsonReader.ReadJson();
            DataTable datatable = JsonReader.DataSet!.Tables[Query.Table]!;
            var row = datatable.NewRow();
            foreach (var val in Query.GetValues())
            {
                row[val.Key] = val.Value;
            }
            datatable.Rows.Add(row);
            Save();
            return 1;
        }
    }
}
