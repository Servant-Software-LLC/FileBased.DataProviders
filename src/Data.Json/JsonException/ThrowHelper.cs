namespace Data.Json.JsonException;
internal static class ThrowHelper
{
    private const string INVALID_JSON = "The json file is not valid";
 
    public static void ThrowIfInvalidJson(JsonDocument jsonDocument,FileConnection jsonConnection)
    {
        if ((jsonDocument.RootElement.ValueKind==JsonValueKind.Object &&
             jsonConnection.PathType==PathType.File )
             ||
             (jsonDocument.RootElement.ValueKind == JsonValueKind.Array &&
             jsonConnection.PathType == PathType.Directory)
             )
        {
            throw new InvalidJsonFileException(INVALID_JSON);
        }
    }
    public static void ThrowIfInvalidJson(JsonElement jsonElement, FileConnection jsonConnection)
    {
        if (!(jsonElement.ValueKind == JsonValueKind.Object &&
             jsonConnection.PathType == PathType.File)
             &&
             !(jsonElement.ValueKind == JsonValueKind.Array &&
             jsonConnection.PathType == PathType.Directory)
             )
        {
            throw new InvalidJsonFileException(INVALID_JSON);
        }
    }

  
}
