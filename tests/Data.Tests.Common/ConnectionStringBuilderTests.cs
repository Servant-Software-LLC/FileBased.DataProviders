using Data.Common.Utils.ConnectionString;
using System.Data.Common;
using Xunit;

namespace Data.Tests.Common;

public static class ConnectionStringBuilderTests
{
    public static void ConnectionStringBuilder_CanBuildAndParse(DbConnectionStringBuilder builder, string dataSource)
    {
        // Arrange
        var fileBuilder = (FileConnectionStringBuilder)builder;
        fileBuilder.DataSource = dataSource;
        fileBuilder.CreateIfNotExist = true;

        // Act - round trip through connection string
        var connectionString = fileBuilder.ConnectionString;
        var newBuilder = (FileConnectionStringBuilder)Activator.CreateInstance(builder.GetType())!;
        newBuilder.ConnectionString = connectionString;

        // Assert
        Assert.Equal(dataSource, newBuilder.DataSource);
        Assert.Equal(true, newBuilder.CreateIfNotExist);
    }
}
