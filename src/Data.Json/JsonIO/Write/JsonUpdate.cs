using Data.Common.FileIO.Write;

namespace Data.Json.JsonIO.Write;

internal class JsonUpdate : FileUpdate
{
    public JsonUpdate(FileUpdateQuery queryParser, FileConnection FileConnection, FileCommand FileCommand) 
        : base(queryParser, FileConnection, FileCommand)
    {
        dataSetWriter = new JsonDataSetWriter(FileConnection, queryParser);
    }
}
