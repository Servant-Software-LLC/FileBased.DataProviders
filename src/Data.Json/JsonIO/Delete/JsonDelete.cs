
using System.Data.JsonClient;

namespace Data.Json.JsonIO.Delete;

internal class JsonDelete : FileDelete<JsonParameter>
{
    public JsonDelete(FileDeleteQuery<JsonParameter> queryParser, FileConnection<JsonParameter> jsonConnection, FileCommand<JsonParameter> jsonCommand) 
        : base(queryParser, jsonConnection, jsonCommand)
    {
        this.dataSetWriter = new JsonDataSetWriter(jsonConnection, queryParser);
    }
}

