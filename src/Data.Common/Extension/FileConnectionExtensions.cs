using Data.Common.DataSource;

namespace Data.Common.Extension;

public static class FileConnectionExtensions
{
    public static DataSourceType GetDataSourceType(this IFileConnection fileConnection)
    {
        if (fileConnection.DataSourceProvider != null)
            return fileConnection.DataSourceProvider.DataSourceType;

        var dataSource = fileConnection.DataSource;
        if (IsAdmin(dataSource))
            return DataSourceType.Admin;

        if (File.Exists(dataSource))
            return DataSourceType.File;
        
        if (Directory.Exists(dataSource))
            return DataSourceType.Directory;

        return DataSourceType.None;
    }

    internal static bool IsAdmin(string database) => string.Compare(database, $":{nameof(DataSourceType.Admin)}:", true) == 0;
}
