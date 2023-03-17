
namespace Data.Json.JsonIO.Delete;

internal class JsonDelete : FileDelete
{
    public JsonDelete(FileDeleteQuery queryParser, FileConnection jsonConnection, FileCommand jsonCommand) : base(queryParser, jsonConnection, jsonCommand)
    {
        this.dataSetWriter = new JsonDataSetWriter(jsonConnection,queryParser);
    }
}

