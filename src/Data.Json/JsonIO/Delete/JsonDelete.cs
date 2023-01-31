using Data.Json.JsonQuery;

namespace Data.Json.JsonIO.Delete
{
    internal class JsonDelete : JsonWriter
    {
        private readonly JsonDeleteQuery jsonDeleteQuery;

        public JsonDelete(JsonCommand command,
            JsonDocument jsonDocument,
            JsonDeleteQuery jsonDeleteQuery)
            : base(command,
                  jsonDocument,
                  jsonDeleteQuery)
        {
            this.jsonDeleteQuery = jsonDeleteQuery;
        }

        public override int Execute()
        {
            DataTable datatable = DataSet.Tables[jsonDeleteQuery.Table]!;
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
