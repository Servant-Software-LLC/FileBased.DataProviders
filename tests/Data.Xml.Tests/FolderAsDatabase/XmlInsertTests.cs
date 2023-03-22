using Data.Common.Extension;
using Data.Tests.Common;
using System.Data.XmlClient;
using System.Reflection;
using Xunit;

namespace Data.Xml.Tests.FolderAsDatabase;

/// <summary>
/// These tests indirectly exercise the <see cref="JsonIO.Write.JsonInsert"/> class via calls to <see cref="XmlCommand.ExecuteNonQuery" />. 
/// </summary>
public class XmlInsertTests
{
    [Fact]
    public void Insert_ShouldInsertData()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        InsertTests.Insert_ShouldInsertData(() => new XmlConnection(ConnectionStrings.Instance.FolderAsDBConnectionString.Sandbox("Sandbox", sandboxId)));
    }

    [Fact]
    public void Insert_JsonShouldBeFormatted()
    {
        // Arrange
        var connection = new XmlConnection(ConnectionStrings.Instance.FolderAsDBConnectionString.AddFormatted(true));
        connection.Open();

        // Act - Insert a new record into the locations table
        const string locationsTableName = "locations";
        var command = new XmlCommand($"INSERT INTO {locationsTableName} (id, city, state, zip) VALUES (156, 'Seattle', 'Washington', 98101)", connection);
        var rowsAffected = command.ExecuteNonQuery();

        // Assert
        var jsonFileContents = File.ReadAllText(connection.GetTablePath(locationsTableName));
        Assert.Contains("\n", jsonFileContents);
    }

}
