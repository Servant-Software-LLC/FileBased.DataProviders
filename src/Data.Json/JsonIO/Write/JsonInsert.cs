
using Data.Common.FileIO.Write;

namespace Data.Json.JsonIO.Write;

internal class JsonInsert : FileInsert
{
    public JsonInsert(FileInsertQuery queryParser, FileConnection jsonConnection, FileCommand jsonCommand) 
        : base(queryParser, jsonConnection, jsonCommand)
    {
        dataSetWriter = new JsonDataSetWriter(jsonConnection, queryParser);
    }

    public override bool SchemaUnknownWithoutData => true;
}
