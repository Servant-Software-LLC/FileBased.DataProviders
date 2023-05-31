namespace Data.Common.Extension;

public static class FileConnectionExtensions
{
    public static string GetTablePath(this IFileConnection fileConnection, string tableName)
    {
        var pathType = GetPathType(fileConnection);
        if (pathType != PathType.Directory)
            throw new InvalidOperationException($"Can only call the {nameof(GetTablePath)} method on a data source of type {nameof(PathType.Directory)}. PathType={pathType}");

        return Path.Combine(fileConnection.Database, $"{tableName}.{fileConnection.FileExtension}");
    }

    internal static PathType GetPathType(this IFileConnection fileConnection)
    {
        if (IsAdmin(fileConnection.Database))
            return PathType.Admin;

        if (File.Exists(fileConnection.Database))
            return PathType.File;
        
        if (Directory.Exists(fileConnection.Database))
            return PathType.Directory;

        return PathType.None;
    }

    internal static bool IsAdmin(string database) => string.Compare(database, $":{nameof(PathType.Admin)}:", true) == 0;
}
