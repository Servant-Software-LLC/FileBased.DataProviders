using Data.Json.JsonQuery;

namespace Data.Json.JsonIO.Write;

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
        //as we have modified the json file so we don't need to update the tables
        jsonConnection.JsonReader.StopWatching();
        _rwLock.EnterWriteLock();

        //Entry try block only after taking a write lock.
        try
        {
            var queryParser = ((JsonUpdateQuery)command.QueryParser);
            var values = queryParser.GetValues();
            DataTable datatable = JsonReader.DataSet!.Tables[queryParser.Table]!;
            DataView dataView = datatable.DefaultView;

            dataView.RowFilter = queryParser.Filter?.Evaluate();
            var rowsAffected = dataView.Count;
            foreach (DataRowView dataRow in dataView)
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
