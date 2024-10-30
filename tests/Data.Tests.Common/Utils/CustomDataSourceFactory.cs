using Data.Common.DataSource;
using Data.Common.Utils.ConnectionString;
using System.Data.FileClient;

namespace Data.Tests.Common.Utils;

public static class CustomDataSourceFactory
{
    public static FileConnection<TFileParameter> VirtualFolderAsDB<TFileParameter>(Func<FileConnectionString, FileConnection<TFileParameter>> createConnection)
            where TFileParameter : FileParameter<TFileParameter>, new()
    {
        var connection = createConnection(FileConnectionString.CustomDataSource);
        var database = new DatabaseFullPaths(connection.FileExtension);
        var folderPath = database.Folder;
        var extension = connection.FileExtension;

        var dataSourceProvider = new StreamedDataSource();
        AddTableToDataSource(folderPath, "employees", extension, dataSourceProvider);
        AddTableToDataSource(folderPath, "locations", extension, dataSourceProvider);

        connection.DataSourceProvider = dataSourceProvider;

        return connection;
    }

    public static void AddTableToDataSource(string folderPath, string tableName, string fileExtension, StreamedDataSource dataSourceProvider)
    {
        var tablePath = Path.Combine(folderPath, $"{tableName}.{fileExtension}");
        var tableFormFile = FormFileUtils.MockFormFile(tablePath, fileExtension);
        dataSourceProvider.AddTable(tableName, tableFormFile.OpenReadStream());
    }
}
