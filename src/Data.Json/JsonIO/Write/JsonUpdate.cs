using Data.Json.JsonQuery;
using Utilities;

namespace Data.Json.JsonIO.Write
{
    internal class JsonUpdate : JsonWriter
    {
        public JsonUpdate(JsonCommand command,
                          JsonConnection jsonConnection) 
            : base(command, jsonConnection)
        {
        }

        public override int Execute()
        {
            JsonReader.ReadJson();
            var queryParser = ((JsonUpdateQuery)command.QueryParser);
            var values = queryParser.GetValues();
            DataTable datatable = JsonReader.DataSet!.Tables[queryParser.Table]!;
            datatable.DefaultView.RowFilter = queryParser.Filter.Evaluate() ;
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
