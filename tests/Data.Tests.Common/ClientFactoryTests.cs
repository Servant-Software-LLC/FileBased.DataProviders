using Data.Tests.Common.Utils;
using System.Data.Common;
using Xunit;

namespace Data.Tests.Common;

public static class ClientFactoryTests
{
    public static void CreateCommand_ReadsData(DbProviderFactory dbProviderFactory)
    {
        // Arrange 

        // This connection uses a DataSourceProvider with table streams (employees and locations)
        // See under the unit tests project's Sources/Folder for these source files.
        var connection = CustomDataSourceFactory.VirtualFolderAsDB(dbProviderFactory);

        connection.Open();
        var command = dbProviderFactory.CreateCommand();
        command!.Connection = connection as DbConnection;
        command.CommandText = "SELECT * FROM employees";

        // Act
        var reader = command.ExecuteReader();

        // Assert
        Assert.True(reader.HasRows);
    }
}
