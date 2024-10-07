using Data.Common.DataSource;

namespace Data.Common.Extension;

public static class FileConnectionExtensions
{
    public static DataSourceType GetDataSourceType(this IFileConnection fileConnection)
    {
        if (IsAdmin(fileConnection.Database))
            return DataSourceType.Admin;

        if (File.Exists(fileConnection.Database))
            return DataSourceType.File;
        
        if (Directory.Exists(fileConnection.Database))
            return DataSourceType.Directory;

        return DataSourceType.None;
    }

    internal static bool IsAdmin(string database) => string.Compare(database, $":{nameof(DataSourceType.Admin)}:", true) == 0;
}
