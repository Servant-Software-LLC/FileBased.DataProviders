namespace Data.Common.Extension;
public static class FileConnectionExtensions
{
    public static string GetTablePath(this FileConnection fileConnection, string tableName)
    {
        var pathType = GetPathType(fileConnection);
        if (pathType != PathType.Directory)
            throw new InvalidOperationException($"Can only call the {nameof(GetTablePath)} method on a data source of type {nameof(PathType.Directory)}. PathType={pathType}");

        return Path.Combine(fileConnection.Database, $"{tableName}.{fileConnection.FileExtension}");
    }

    internal static PathType GetPathType(this FileConnection jsonConnection)
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
