using Data.Json.JsonQuery;

namespace Data.Json.JsonIO.Delete;

internal class JsonDelete : JsonWriter
{
    internal readonly JsonDeleteQuery Query;

    public JsonDelete(JsonDeleteQuery queryParser, JsonConnection jsonConnection,JsonCommand jsonCommand)
        : base(jsonConnection, jsonCommand, queryParser)
    {
        this.Query = queryParser ?? throw new ArgumentNullException(nameof(queryParser));
    }

    public override int Execute()
    {
        try
        {
            //as we have modified the json file so we don't need to update the tables
            jsonReader.StopWatching();
            _rwLock.EnterWriteLock();

            var dataTable = jsonReader.ReadJson(Query);

            //Create a DataView to work with just for this operation
            var dataView = new DataView(dataTable);
            dataView.RowFilter = Query.Filter?.ToString();

            var rowsAffected = dataView.Count;
            //don't update now if it is a transaction
            if (base.IsTransaction)
            {
                jsonTransaction!.Writers.Add(this);
                return rowsAffected;
            }
            foreach (DataRowView dataRow in dataView)
            {
                dataTable!.Rows.Remove(dataRow.Row);
            }
          
            return rowsAffected;
        }
        finally
        {
            jsonReader.StartWatching();
            Save();
            _rwLock.ExitWriteLock();
        }
    }
}

