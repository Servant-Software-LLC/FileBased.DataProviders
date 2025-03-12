using Data.Tests.Common.Utils;
using System.Data.Common;
using Xunit;

namespace Data.Tests.Common;

public static class ClientFactoryTests
{
    public static void CreateCommand_ReadsData(DbProviderFactory dbProviderFactory)
    {
        // Arrange
        var connection = CustomDataSourceFactory.VirtualFolderAsDB(dbProviderFactory, false);

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
