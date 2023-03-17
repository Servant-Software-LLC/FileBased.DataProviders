
using Data.Common.FileIO.Write;
using Data.Common.FileQuery;
using System.Data.FileClient;

namespace Data.Json.JsonIO.Write;

internal class JsonInsert : FileInsert
{
    public JsonInsert(FileInsertQuery queryParser, FileConnection jsonConnection, FileCommand jsonCommand) : base(queryParser, jsonConnection, jsonCommand)
    {
        this.dataSetWriter = new JsonDataSetWriter(jsonConnection, queryParser);

    }
}
