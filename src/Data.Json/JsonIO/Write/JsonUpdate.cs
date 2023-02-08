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
            try
            {
                JsonReader.ReadJson();
                //as we have modified the json file so we don't need to update the tables
                jsonConnection.JsonReader.StopWatching();
                _rwLock.EnterWriteLock();
                var queryParser = ((JsonUpdateQuery)command.QueryParser);
                var values = queryParser.GetValues();
                DataTable datatable = JsonReader.DataSet!.Tables[queryParser.Table]!;
                datatable.DefaultView.RowFilter = queryParser.Filter?.Evaluate();
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
            finally
            {
                _rwLock.ExitWriteLock();
                jsonConnection.JsonReader.StartWatching();
            }

        }
    }
}
