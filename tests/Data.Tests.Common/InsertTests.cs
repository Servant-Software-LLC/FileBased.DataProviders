using Data.Common.Extension;
using System.Data.FileClient;
using Xunit;

namespace Data.Tests.Common;

public static class InsertTests
{
    public static void Insert_ShouldInsertData(Func<FileConnection> createFileConnection)
    {
        const int id = 123054;

        // Arrange
        using (var connection = createFileConnection())
        {
            connection.Open();

            // Act - Insert a new record into the locations table
            var command = connection.CreateCommand($"INSERT INTO locations (id, city, state, zip) VALUES ({id}, 'Seattle', 'Washington', 98101)");
            var rowsAffected = command.ExecuteNonQuery();

            // Assert
            Assert.Equal(1, rowsAffected);
            connection.Open();

            // Act - Verify the inserted record exists in the locations table
            command = connection.CreateCommand($"SELECT COUNT(*) FROM locations WHERE id = {id}");
            var count = (int)command.ExecuteScalar()!;

            // Assert
            Assert.True(1 == count, $"SELECTing COUNT(*) for id = {id} didn't yield 1 row.  Rows = {count}");

            // Act - Insert a new record into the employees table
            command = connection.CreateCommand("INSERT INTO employees (name, email, salary, married) VALUES ('Jim Convis', 'johndoe@example.com', 100000, 'true')");
            rowsAffected = command.ExecuteNonQuery();

            // Assert
            Assert.True(1 == rowsAffected, $"{nameof(command.ExecuteNonQuery)} indicated its rows affected wasn't 1.  RowsAffected = {rowsAffected}");

            // Close the connection
            connection.Close();
        }
    }

    public static void Insert_ShouldBeFormatted(Func<FileConnection> createFileConnection)
    {
        // Arrange
        var connection = createFileConnection();
        connection.Open();

        // Act - Insert a new record into the locations table
        const string locationsTableName = "locations";
        var command = connection.CreateCommand($"INSERT INTO {locationsTableName} (id, city, state, zip) VALUES (156, 'Seattle', 'Washington', 98101)");
        var rowsAffected = command.ExecuteNonQuery();

        // Assert
        var jsonFileContents = File.ReadAllText(connection.GetTablePath(locationsTableName));
        Assert.Contains("\n", jsonFileContents);
    }

    public static void Insert_ShouldBeFormattedForFile(Func<FileConnection> createFileConnection)
    {
        // Arrange
        var connection = createFileConnection();
        connection.Open();

        // Act - Insert a new record into the locations table
        const string locationsTableName = "locations";
        var command = connection.CreateCommand($"INSERT INTO {locationsTableName} (id, city, state, zip) VALUES (156, 'Seattle', 'Washington', 98101)");
        var rowsAffected = command.ExecuteNonQuery();

        // Assert
        var jsonFileContents = File.ReadAllText(connection.Database);
        Assert.Contains("\n", jsonFileContents);
    }
}