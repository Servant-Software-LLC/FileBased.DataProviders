using Data.Common.FileIO.Create;
using System.Data.JsonClient;

namespace Data.Json.JsonIO.Create;

internal class JsonCreateTable : FileCreateTableWriter
{
    public JsonCreateTable(FileCreateTable fileStatement, FileConnection<JsonParameter> fileConnection, FileCommand<JsonParameter> fileCommand)
        : base(fileStatement, fileConnection, fileCommand)
    {
    }
}
