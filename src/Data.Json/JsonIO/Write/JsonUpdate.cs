using Data.Common.FileIO.Write;
using System.Data.JsonClient;

namespace Data.Json.JsonIO.Write;

internal class JsonUpdate : FileUpdateWriter
{
    public JsonUpdate(FileUpdate fileStatement, FileConnection<JsonParameter> FileConnection, FileCommand<JsonParameter> FileCommand)
        : base(fileStatement, FileConnection, FileCommand)
    {
        dataSetWriter = new JsonDataSetWriter(FileConnection, fileStatement);
    }
}
