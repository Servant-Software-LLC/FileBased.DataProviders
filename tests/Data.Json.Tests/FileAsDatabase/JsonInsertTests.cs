using System.Data.JsonClient;
using Xunit;

namespace Data.Json.Tests.FileAsDatabase;

public class JsonInsertTests
{
    [Fact]
    public void Insert_ShouldInsertData()
    {
        // Arrange
        var connection = new JsonConnection(ConnectionStrings.FileAsDBConnectionString);
        connection.Open();

        // Act - Insert a new record into the locations table
        var command = new JsonCommand("INSERT INTO locations (id, city, state, zip) VALUES (156, 'Seattle', 'Washington', 98101)", connection);
        var rowsAffected = command.ExecuteNonQuery();

        // Assert
        Assert.Equal(1, rowsAffected);


        // Act - Insert a new record into the employees table
        connection = new JsonConnection(ConnectionStrings.FileAsDBConnectionString);
        connection.Open();
        command = new JsonCommand("INSERT INTO employees (name, email, salary, married) VALUES ('Haleem', 'johndoe@example.com', 100000, 'true')", connection);
        rowsAffected = command.ExecuteNonQuery();

        // Assert
        Assert.Equal(1, rowsAffected);
        // Close the connection
        connection.Close();
    }

    [Fact]
    public void Insert_JsonShouldBeFormatted()
    {
        // Arrange
        var connection = new JsonConnection(ConnectionStrings.AddFormatted(ConnectionStrings.FileAsDBConnectionString, true));
        connection.Open();

        // Act - Insert a new record into the locations table
        var command = new JsonCommand("INSERT INTO locations (id, city, state, zip) VALUES (156, 'Seattle', 'Washington', 98101)", connection);
        var rowsAffected = command.ExecuteNonQuery();

        // Assert
        var jsonFileContents = File.ReadAllText(connection.Database);
        Assert.Contains("\n", jsonFileContents);
    }

}
