namespace Data.Json.Extension;
internal static class JsonElementExtensions
{
    internal static object? GetValue(this JsonElement jsonElement)
    {
        var kind = jsonElement.ValueKind;
        return kind switch
        {
            JsonValueKind.String => jsonElement.GetString(),
            JsonValueKind.Number => jsonElement.GetDecimal(),
            JsonValueKind.Undefined => string.Empty,
            JsonValueKind.Array => jsonElement.GetRawText(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Object => jsonElement.GetRawText(),
            JsonValueKind.Null => null,
            _ => throw new NotImplementedException()
        };

    }
}
