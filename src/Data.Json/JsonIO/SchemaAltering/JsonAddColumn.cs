using Data.Common.FileIO.SchemaAltering;
using System.Data.JsonClient;

namespace Data.Json.JsonIO.SchemaAltering;

internal class JsonAddColumn : FileAddColumnWriter
{
    public JsonAddColumn(FileAddColumn fileStatement, FileConnection<JsonParameter> fileConnection, FileCommand<JsonParameter> fileCommand)
        : base(fileStatement, fileConnection, fileCommand)
    {
    }
}