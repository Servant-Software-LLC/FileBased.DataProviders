using Data.Common.FileIO.Delete;
using System.Data.JsonClient;

namespace Data.Json.JsonIO.Delete;

internal class JsonDelete : FileDeleteWriter
{
    public JsonDelete(FileDelete fileStatement, FileConnection<JsonParameter> jsonConnection, FileCommand<JsonParameter> jsonCommand)
        : base(fileStatement, jsonConnection, jsonCommand)
    {
        dataSetWriter = new JsonDataSetWriter(jsonConnection, fileStatement);
    }
}

