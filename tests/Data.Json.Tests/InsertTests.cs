using System.Data.JsonClient;
using Xunit;

namespace Data.Json.Tests;

public class InsertTests
{
    #region Folder as database
    [Fact]
    public void Insert_ShouldInsertData()
    {
        // Arrange

        var connection = new JsonConnection(ConnectionStrings.FolderAsDBConnectionString);
        connection.Open();

        // Act - Insert a new record into the locations table
        var command = new JsonCommand("INSERT INTO locations (id, city, state, zip) VALUES (1000, 'Seattle', 'Washington', 98101)", connection);
        var rowsAffected = command.ExecuteNonQuery();

        // Assert
        Assert.Equal(1, rowsAffected);
        connection.Close();
        connection.Dispose();
        connection = new JsonConnection(ConnectionStrings.FolderAsDBConnectionString);
        connection.Open();
        // Act - Verify the inserted record exists in the locations table
        command = new JsonCommand("SELECT COUNT(*) FROM locations WHERE id = 1000", connection);
        var count = (int)command.ExecuteScalar();

        // Assert
        Assert.Equal(1, count);
        connection.Close();
        connection.Dispose();
        connection = new JsonConnection(ConnectionStrings.FolderAsDBConnectionString);
        connection.Open();
        // Act - Insert a new record into the employees table
        command = new JsonCommand("INSERT INTO employees (name, email, salary, married) VALUES ('Jim Convis', 'johndoe@example.com', 100000, 'true')", connection);
        rowsAffected = command.ExecuteNonQuery();

        // Assert
        Assert.Equal(1, rowsAffected);
        // Close the connection
        connection.Close();
    }
    #endregion

    #region File as database

    [Fact]
    public void Insert_ShouldInsertDataIntoFile()
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
    #endregion
}
