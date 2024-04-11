using Data.Common.FileIO.SchemaAltering;
using System.Data.JsonClient;

namespace Data.Json.JsonIO.SchemaAltering;

internal class JsonDropColumn : FileDropColumnWriter
{
    public JsonDropColumn(FileDropColumn fileStatement, FileConnection<JsonParameter> fileConnection, FileCommand<JsonParameter> fileCommand)
        : base(fileStatement, fileConnection, fileCommand)
    {
    }
}