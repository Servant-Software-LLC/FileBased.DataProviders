using Data.Common.DataSource;
using Data.Common.Interfaces;
using Data.Common.Utils.ConnectionString;
using System.Data.Common;
using System.Data.FileClient;

namespace Data.Tests.Common.Utils;

public static class CustomDataSourceFactory
{
    public static FileConnection<TFileParameter> VirtualFolderAsDB<TFileParameter>(Func<FileConnectionString, FileConnection<TFileParameter>> createConnection, bool largeFolder = false)
            where TFileParameter : FileParameter<TFileParameter>, new()
    {
        var connection = createConnection(FileConnectionString.CustomDataSource);
        return (FileConnection<TFileParameter>)SetupConnection(connection, largeFolder);
    }

    public static IFileConnection VirtualFolderAsDB(DbProviderFactory dbProviderFactory, bool largeFolder)
    {
        var connection = dbProviderFactory.CreateConnection() as IFileConnection;
        return SetupConnection(connection!, largeFolder);
    }

    public static IFileConnection SetupConnection(IFileConnection connection, bool largeFolder)
    {
        var database = new DatabaseFullPaths(connection.FileExtension);
        var folderPath = largeFolder ? database.LargeFolder : database.Folder;
        var extension = connection.FileExtension;

        var dataSourceProvider = new TableStreamedDataSource("MyDatabase");
        AddTableToDataSource(folderPath, "employees", extension, dataSourceProvider);
        AddTableToDataSource(folderPath, "locations", extension, dataSourceProvider);

        connection.DataSourceProvider = dataSourceProvider;

        return connection;
    }

    public static void AddTableToDataSource(string folderPath, string tableName, string fileExtension, TableStreamedDataSource dataSourceProvider)
    {
        dataSourceProvider.AddTable(tableName, () =>
            {
                var tablePath = Path.Combine(folderPath, $"{tableName}.{fileExtension}");
                var tableFormFile = FormFileUtils.MockFormFile(tablePath, fileExtension);
                return tableFormFile.OpenReadStream();
            }
        );
    }   
}
