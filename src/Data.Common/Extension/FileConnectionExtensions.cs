namespace Data.Common.Extension;

public static class FileConnectionExtensions
{
    public static string GetTablePath<TFileParameter>(this FileConnection<TFileParameter> fileConnection, string tableName)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        var pathType = GetPathType(fileConnection);
        if (pathType != PathType.Directory)
            throw new InvalidOperationException($"Can only call the {nameof(GetTablePath)} method on a data source of type {nameof(PathType.Directory)}. PathType={pathType}");

        return Path.Combine(fileConnection.Database, $"{tableName}.{fileConnection.FileExtension}");
    }

    internal static PathType GetPathType<TFileParameter>(this FileConnection<TFileParameter> fileConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
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
