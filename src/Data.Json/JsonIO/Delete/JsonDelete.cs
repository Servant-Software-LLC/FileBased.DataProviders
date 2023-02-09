using Data.Json.JsonQuery;

namespace Data.Json.JsonIO.Delete;

internal class JsonDelete : JsonWriter
{
    private readonly JsonDeleteQuery jsonDeleteQuery;

    public JsonDelete(JsonCommand command,
        JsonConnection jsonConnection)
        : base(command,
              jsonConnection)
    {
        jsonDeleteQuery = (JsonDeleteQuery)command.QueryParser;
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
            DataTable dataTable = JsonReader.DataTable;
            DataView dataView = dataTable.DefaultView;
            dataView.RowFilter = jsonDeleteQuery.Filter?.ToString();
            var rowsAffected = 0;
            foreach (DataRowView dataRow in dataView)
            {
                dataTable.Rows.Remove(dataRow.Row);
                rowsAffected++;
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

