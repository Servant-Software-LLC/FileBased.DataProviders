
using Data.Common.FileIO.Write;
using Data.Common.FileQuery;
using System.Data.FileClient;
using System.Data.JsonClient;

namespace Data.Json.JsonIO.Write;

internal class JsonUpdate : FileUpdate
{
    public JsonUpdate(FileUpdateQuery queryParser, FileConnection FileConnection, FileCommand FileCommand) : base(queryParser, FileConnection, FileCommand)
    {
        this.dataSetWriter = new JsonDataSetWriter(FileConnection, queryParser);

    }
}
