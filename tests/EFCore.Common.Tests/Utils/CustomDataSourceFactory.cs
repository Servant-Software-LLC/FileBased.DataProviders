using Data.Common.DataSource;
using Data.Tests.Common.Utils;

namespace EFCore.Common.Tests.Utils;

public static class CustomDataSourceFactory
{
    public static IDataSourceProvider VirtualGettingStartedFolderAsDB(string extension) =>
        VirtualFolderAsDB(extension, new DatabaseFullPaths(extension).gettingStartedFolderDataBase);

    public static IDataSourceProvider VirtualGettingStartedWithDataFolderAsDB(string extension) =>
        VirtualFolderAsDB(extension, new DatabaseFullPaths(extension).gettingStartedWithDataFolderDataBase);

    private static IDataSourceProvider VirtualFolderAsDB(string extension, string folderPath)
    {
        var dataSourceProvider = new StreamedDataSource();
        Data.Tests.Common.Utils.CustomDataSourceFactory.AddTableToDataSource(folderPath, "Blogs", extension, dataSourceProvider);
        Data.Tests.Common.Utils.CustomDataSourceFactory.AddTableToDataSource(folderPath, "Posts", extension, dataSourceProvider);

        return dataSourceProvider;
    }
}
