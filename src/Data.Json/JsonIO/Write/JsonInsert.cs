using Data.Json.JsonQuery;

namespace Data.Json.JsonIO.Write;

internal class JsonInsert : JsonWriter
{
    private readonly JsonInsertQuery queryParser;
    public JsonInsert(JsonInsertQuery queryParser, JsonConnection jsonConnection, JsonCommand jsonCommand)
        : base(jsonConnection, jsonCommand,queryParser)
    {
        this.queryParser = queryParser ?? throw new ArgumentNullException(nameof(queryParser));
    }


    public override int Execute()
    {
        if (base.IsTransaction)
        {
            jsonTransaction!.Writers.Add(this);
            return 1;
        }
        try
        {
           
            //as we have modified the json file so we don't need to update the tables
            jsonReader.StopWatching();
            _rwLock.EnterWriteLock();
            var dataTable = jsonReader.ReadJson(queryParser);
            var row = dataTable!.NewRow();
            foreach (var val in queryParser.GetValues())
            {
                row[val.Key] = val.Value;
            }
                dataTable.Rows.Add(row);
        }
        finally
        {
            Save();
            _rwLock.ExitWriteLock();
            jsonReader.StartWatching();
        }
        return 1;
    }
  

}
