namespace Data.Common.Extension;

public static class FileConnectionExtensions
{
    /// <summary>
    /// Provides a full file path to the table file.
    /// </summary>
    /// <param name="fileConnection"></param>
    /// <param name="tableName">Name of table</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static string GetTablePath(this IFileConnection fileConnection, string tableName)
    {
        var pathType = GetPathType(fileConnection);
        if (pathType != PathType.Directory)
            throw new InvalidOperationException($"Can only call the {nameof(GetTablePath)} method on a data source of type {nameof(PathType.Directory)}. PathType={pathType}");

        return Path.Combine(fileConnection.Database, GetTableFileName(fileConnection, tableName));
    }

    public static string GetTableFileName(this IFileConnection fileConnection, string tableName) =>
        $"{tableName}.{fileConnection.FileExtension}";

    public static PathType GetPathType(this IFileConnection fileConnection)
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
