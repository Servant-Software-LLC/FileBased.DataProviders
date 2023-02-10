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
            try
            {
                //as we have modified the json file so we don't need to update the tables
                jsonConnection.JsonReader.StopWatching();
                _rwLock.EnterWriteLock();
                JsonReader.ReadJson();
                DataTable datatable = JsonReader.DataSet!.Tables[Query.Table]!;
                var row = datatable.NewRow();
                foreach (var val in Query.GetValues())
                {
                    row[val.Key] = val.Value;
                }
                datatable.Rows.Add(row);
                Save();
            }
            finally
            {
                _rwLock.ExitWriteLock();
                jsonConnection.JsonReader.StartWatching();
            }
            return 1;
        }
    }
}
