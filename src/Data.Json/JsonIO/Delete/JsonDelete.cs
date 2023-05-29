using System.Data.JsonClient;

namespace Data.Json.JsonIO.Delete;

internal class JsonDelete : FileDelete
{
    public JsonDelete(FileDeleteQuery queryParser, FileConnection<JsonParameter> jsonConnection, FileCommand<JsonParameter> jsonCommand) 
        : base(queryParser, jsonConnection, jsonCommand)
    {
        this.dataSetWriter = new JsonDataSetWriter(jsonConnection, queryParser);
    }
}

