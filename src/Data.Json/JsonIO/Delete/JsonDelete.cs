using Data.Json.JsonQuery;

namespace Data.Json.JsonIO.Delete
{
    internal class JsonDelete : JsonWriter
    {
        private readonly JsonDeleteQuery jsonDeleteQuery;

        public JsonDelete(JsonCommand command,
            JsonConnection jsonConnection)
            : base(command,
                  jsonConnection)
        {
            this.jsonDeleteQuery = (JsonDeleteQuery)command.QueryParser;
        }

        public override int Execute()
        {
            JsonReader.ReadJson();
            DataTable datatable =JsonReader.DataSet!.Tables[jsonDeleteQuery.Table]!;
            datatable.DefaultView.RowFilter = jsonDeleteQuery.Filter?.ToString();
            var rowsAffected = datatable.DefaultView.Count;
            foreach (DataRowView dataRow in datatable.DefaultView)
            {
                datatable.Rows.Remove(dataRow.Row);
            }
            Save();
            return rowsAffected;
        }
    }
}
