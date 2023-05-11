using Data.Common.FileIO.Write;
using System.Data.JsonClient;

namespace Data.Json.JsonIO.Write;

internal class JsonUpdate : FileUpdate<JsonParameter>
{
    public JsonUpdate(FileUpdateQuery<JsonParameter> queryParser, FileConnection<JsonParameter> FileConnection, FileCommand<JsonParameter> FileCommand) 
        : base(queryParser, FileConnection, FileCommand)
    {
        dataSetWriter = new JsonDataSetWriter(FileConnection, queryParser);
    }
}
