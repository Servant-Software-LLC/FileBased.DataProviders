using Data.Common.DataSource;
using Data.Common.Utils.ConnectionString;
using System.Data.FileClient;

namespace Data.Tests.Common.Utils;

public static class CustomDataSourceFactory
{
    public static FileConnection<TFileParameter> CreateFolder<TFileParameter>(Func<FileConnectionString, FileConnection<TFileParameter>> createConnection, ConnectionStringsBase connectionStringsBase)
            where TFileParameter : FileParameter<TFileParameter>, new()
    {
        var connection = createConnection(FileConnectionString.CustomDataSource);
        var database = new DatabaseFullPaths(connection.FileExtension);
        var folderPath = database.Folder;
        var employeesTableName = "employees";
        var extension = connection.FileExtension;
        var employeesPath = Path.Combine(folderPath, $"{employeesTableName}.{extension}");
        var employeesFormFile = FormFileUtils.MockFormFile(employeesPath, extension);
        var locationsTableName = "locations";
        var locationsPath = Path.Combine(folderPath, $"{locationsTableName}.{extension}");
        var locationsFormFile = FormFileUtils.MockFormFile(locationsPath, extension);
        var dataSourceProvider = new StreamedDataSource(employeesTableName, employeesFormFile.OpenReadStream());
        dataSourceProvider.AddTable(locationsTableName, locationsFormFile.OpenReadStream());
        connection.DataSourceProvider = dataSourceProvider;

        return connection;

    }
}
