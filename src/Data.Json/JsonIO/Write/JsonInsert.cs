using Data.Common.FileIO.Write;
using System.Data.JsonClient;

namespace Data.Json.JsonIO.Write;

internal class JsonInsert : FileInsert<JsonParameter>
{
    public JsonInsert(FileInsertQuery<JsonParameter> queryParser, FileConnection<JsonParameter> jsonConnection, FileCommand<JsonParameter> jsonCommand) 
        : base(queryParser, jsonConnection, jsonCommand)
    {
        dataSetWriter = new JsonDataSetWriter(jsonConnection, queryParser);
    }

    public override bool SchemaUnknownWithoutData => true;
}
