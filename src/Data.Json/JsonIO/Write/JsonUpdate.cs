namespace Data.Json.JsonIO.Write
{
    internal class JsonUpdate : JsonWriter
    {
        public JsonUpdate(JsonCommand command, JsonDocument jsonDocument, JsonQueryParser jSONQuery) : base(command, jsonDocument, jSONQuery)
        {
        }

        public override int Execute()
        {
            var queryParser = ((JsonUpdateQuery)base.queryParser);
            var values = queryParser.GetValues();
            DataTable datatable = DataSet.Tables[queryParser.Table]!;
            var rowsAffected = datatable.DefaultView.Count;
            foreach (DataRowView dataRow in datatable.DefaultView)
            {
                foreach (var val in values)
                {
                    dataRow[val.Key] = val.Value;
                }
            }
            Save();
            return rowsAffected;
        }
    }
}
