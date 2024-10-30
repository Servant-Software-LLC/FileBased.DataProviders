using Data.Common.DataSource;
using Data.Tests.Common.Utils;

namespace EFCore.Common.Tests.Utils;

public static class CustomDataSourceFactory
{
    public static IDataSourceProvider VirtualGettingStartedFolderAsDB(string extension)
    {
        var database = new DatabaseFullPaths(extension);
        var folderPath = database.gettingStartedFolderDataBase;

        var dataSourceProvider = new StreamedDataSource();
        Data.Tests.Common.Utils.CustomDataSourceFactory.AddTableToDataSource(folderPath, "Blogs", extension, dataSourceProvider);
        Data.Tests.Common.Utils.CustomDataSourceFactory.AddTableToDataSource(folderPath, "Posts", extension, dataSourceProvider);

        return dataSourceProvider;
    }
}
