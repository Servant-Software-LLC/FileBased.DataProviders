using Data.Json.JsonQuery;

namespace Data.Json.JsonIO.Write;

internal class JsonUpdate : JsonWriter
{
    private readonly JsonUpdateQuery queryParser;

    public JsonUpdate(JsonUpdateQuery queryParser, JsonConnection jsonConnection, JsonCommand jsonCommand) 
        : base(jsonConnection, jsonCommand, queryParser)
    {
        this.queryParser = queryParser ?? throw new ArgumentNullException(nameof(queryParser));
    }

    public override int Execute()
    {
        try
        {
            // As we have modified the json file so we don't need to update the tables
            jsonReader.StopWatching();
            _rwLock.EnterWriteLock();

            var dataTable = jsonReader.ReadJson(queryParser);
            var values = queryParser.GetValues();

            //Create a DataView to work with just for this operation
            var dataView = new DataView(dataTable);
            dataView.RowFilter = queryParser.Filter?.Evaluate();

            var rowsAffected = dataView.Count;
            //don't update now if it is a transaction
            if (base.IsTransaction)
            {
                jsonTransaction!.Writers.Add(this);
                return rowsAffected;
            }
            foreach (DataRowView dataRow in dataView)
            {
                foreach (var val in values)
                {
                    dataRow[val.Key] = val.Value;
                }
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
