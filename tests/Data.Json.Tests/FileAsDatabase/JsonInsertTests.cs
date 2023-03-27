using Data.Common.Extension;
using Data.Tests.Common;
using System.Data.JsonClient;
using System.Reflection;
using Xunit;

namespace Data.Json.Tests.FileAsDatabase;

public class JsonInsertTests
{
    [Fact]
    public void Insert_ShouldInsertData()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        InsertTests.Insert_ShouldInsertData(() => new JsonConnection(ConnectionStrings.Instance.FileAsDBConnectionString.Sandbox("Sandbox", sandboxId)));
    }

    [Fact]
    public void Insert_JsonShouldBeFormatted()
    {
        // Arrange
        var connection = new JsonConnection(ConnectionStrings.Instance.FileAsDBConnectionString.AddFormatted(true));
        connection.Open();

        // Act - Insert a new record into the locations table
        var command = new JsonCommand("INSERT INTO locations (id, city, state, zip) VALUES (156, 'Seattle', 'Washington', 98101)", connection);
        var rowsAffected = command.ExecuteNonQuery();

        // Assert
        var jsonFileContents = File.ReadAllText(connection.Database);
        Assert.Contains("\n", jsonFileContents);
    }

}
