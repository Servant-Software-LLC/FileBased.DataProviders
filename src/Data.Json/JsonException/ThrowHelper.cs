using Data.Common.DataSource;

namespace Data.Json.JsonException;

internal static class ThrowHelper
{
    private const string INVALID_JSON = "The json file is not valid";
 
    public static void ThrowIfInvalidJson(JsonElement jsonElement, IFileConnection jsonConnection)
    {
        if (!(jsonElement.ValueKind == JsonValueKind.Object &&
             jsonConnection.DataSourceType == DataSourceType.File)
             &&
             !(jsonElement.ValueKind == JsonValueKind.Array &&
             jsonConnection.DataSourceType == DataSourceType.Directory)
             )
        {
            throw new InvalidJsonFileException(INVALID_JSON);
        }
    }

  
}
