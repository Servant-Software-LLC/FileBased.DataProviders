using Data.Json.Enum;

namespace Data.Json.Extension;

public static class JsonConnectionExtensions
{
    public static string GetTablePath(this JsonConnection jsonConnection, string tableName)
    {
        var pathType = GetPathType(jsonConnection);
        if (pathType != PathType.Directory)
            throw new InvalidOperationException($"Can only call the {nameof(GetTablePath)} method on a data source of type {nameof(PathType.Directory)}. PathType={pathType}");

        return Path.Combine(jsonConnection.Database, $"{tableName}.json");
    }

    internal static PathType GetPathType(this JsonConnection jsonConnection)
    {
        if (File.Exists(jsonConnection.Database))
        {
            return PathType.File;
        }
        else if (Directory.Exists(jsonConnection.Database))
        {
            return PathType.Directory;
        }

        return PathType.None;
    }

}
