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
        InsertTests.Insert_ShouldInsertData(() => new JsonConnection(ConnectionStrings.Instance.FileAsDB.Sandbox("Sandbox", sandboxId)));
    }

    //This is a special case.  In json, if a table does not have any rows in it, then we have no schema information on the columns or their data types.
    //Inserting the first row into this table will then determine the columns (along with their data types) in this table.
    [Fact]
    public void Insert_ShouldInsertDataIntoEmptyTables()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        InsertTests.Insert_ShouldInsertData(() => new JsonConnection(ConnectionStrings.Instance.FileAsDBEmptyWithTables.Sandbox("Sandbox", sandboxId)));
    }

    [Fact]
    public void Insert_JsonShouldBeFormatted()
    {
        // Arrange
        var connection = new JsonConnection(ConnectionStrings.Instance.FileAsDB.AddFormatted(true));
        connection.Open();

        // Act - Insert a new record into the locations table
        var command = new JsonCommand("INSERT INTO locations (id, city, state, zip) VALUES (156, 'Seattle', 'Washington', 98101)", connection);
        var rowsAffected = command.ExecuteNonQuery();

        // Assert
        var jsonFileContents = File.ReadAllText(connection.Database);
        Assert.Contains("\n", jsonFileContents);
    }

}
