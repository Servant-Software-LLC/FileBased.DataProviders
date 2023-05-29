using Data.Common.FileIO.Write;
using System.Data.JsonClient;

namespace Data.Json.JsonIO.Write;

internal class JsonUpdate : FileUpdate
{
    public JsonUpdate(FileUpdateQuery queryParser, FileConnection<JsonParameter> FileConnection, FileCommand<JsonParameter> FileCommand) 
        : base(queryParser, FileConnection, FileCommand)
    {
        dataSetWriter = new JsonDataSetWriter(FileConnection, queryParser);
    }
}
