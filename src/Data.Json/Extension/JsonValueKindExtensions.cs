namespace Data.Json.Extension;

internal static class JsonValueKindExtensions
{
    internal static Type GetClrFieldType(this JsonValueKind kind, IFileConnection fileConnection) => kind switch
    {
        JsonValueKind.String => typeof(string),
        JsonValueKind.Number => fileConnection.PreferredFloatingPointDataType.ToType(),
        JsonValueKind.True => typeof(bool),
        JsonValueKind.False => typeof(bool),
        JsonValueKind.Null => typeof(string),
        JsonValueKind.Array => typeof(string),
        JsonValueKind.Object => typeof(string),

        _ => throw new NotSupportedException($"Unsupported JSON value kind: {kind}")
    };
}
