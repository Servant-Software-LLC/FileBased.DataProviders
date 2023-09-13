using Data.Common.FileIO.Create;
using System.Data.JsonClient;

namespace Data.Json.JsonIO.Create;

internal class JsonCreateTable : FileCreateTableWriter
{
    public JsonCreateTable(FileCreateTable fileStatement, FileConnection<JsonParameter> FileConnection, FileCommand<JsonParameter> FileCommand)
        : base(fileStatement, FileConnection, FileCommand)
    {
        dataSetWriter = new JsonDataSetWriter(FileConnection, fileStatement);
    }
}
