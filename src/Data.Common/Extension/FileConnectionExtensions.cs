using Data.Common.DataSource;

namespace Data.Common.Extension;

public static class FileConnectionExtensions
{
    public static DataSourceType GetDataSourceType(this IFileConnection fileConnection)
    {
        if (fileConnection.DataSourceProvider != null)
            return fileConnection.DataSourceProvider.DataSourceType;

        var database = fileConnection.Database;
        if (IsAdmin(database))
            return DataSourceType.Admin;

        if (File.Exists(database))
            return DataSourceType.File;
        
        if (Directory.Exists(database))
            return DataSourceType.Directory;

        return DataSourceType.None;
    }

    internal static bool IsAdmin(string database) => string.Compare(database, $":{nameof(DataSourceType.Admin)}:", true) == 0;
}
