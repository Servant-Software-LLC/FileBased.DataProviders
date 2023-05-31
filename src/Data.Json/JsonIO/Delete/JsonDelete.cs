using System.Data.JsonClient;

namespace Data.Json.JsonIO.Delete;

internal class JsonDelete : Common.FileIO.Delete.FileDeleteWriter
{
    public JsonDelete(Common.FileStatements.FileDelete fileStatement, FileConnection<JsonParameter> jsonConnection, FileCommand<JsonParameter> jsonCommand) 
        : base(fileStatement, jsonConnection, jsonCommand)
    {
        this.dataSetWriter = new JsonDataSetWriter(jsonConnection, fileStatement);
    }
}

