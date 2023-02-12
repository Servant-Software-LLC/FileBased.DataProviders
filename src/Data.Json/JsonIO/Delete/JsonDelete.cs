using Data.Json.JsonQuery;

namespace Data.Json.JsonIO.Delete;

internal class JsonDelete : JsonWriter
{
    private readonly JsonDeleteQuery queryParser;

    public JsonDelete(JsonDeleteQuery queryParser, JsonConnection jsonConnection)
        : base(jsonConnection)
    {
        this.queryParser = queryParser ?? throw new ArgumentNullException(nameof(queryParser));
    }

    public override int Execute()
    {
        try
        {
            //as we have modified the json file so we don't need to update the tables
            jsonReader.StopWatching();
            _rwLock.EnterWriteLock();

            var dataTable = jsonReader.ReadJson(queryParser);

            //Create a DataView to work with just for this operation
            var dataView = new DataView(dataTable);
            dataView.RowFilter = queryParser.Filter?.ToString();

            var rowsAffected = dataView.Count;
            foreach (DataRowView dataRow in dataView)
            {
                dataTable!.Rows.Remove(dataRow.Row);
            }
          
            return rowsAffected;
        }
        finally
        {
            jsonReader.StartWatching();
            Save(queryParser.TableName);
            _rwLock.ExitWriteLock();
        }
    }
}

