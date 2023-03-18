using Data.Common.Extension;
using System.Data.XmlClient;
using Xunit;

namespace Data.Xml.Tests.FolderAsDatabase;

/// <summary>
/// These tests indirectly exercise the <see cref="JsonIO.Write.JsonInsert"/> class via calls to <see cref="CsvCommand.ExecuteNonQuery" />. 
/// </summary>
public class CsvInsertTests
{
    [Fact]
    public void Insert_ShouldInsertData()
    {
        const int id = 123054;

        // Arrange
        var connection = new CsvConnection(ConnectionStrings.FolderAsDBConnectionString);
        connection.Open();

        // Act - Insert a new record into the locations table
        var command = new CsvCommand($"INSERT INTO locations (id, city, state, zip) VALUES ({id}, 'Seattle', 'Washington', 98101)", connection);
        var rowsAffected = command.ExecuteNonQuery();

        // Assert
        Assert.Equal(1, rowsAffected);
        connection.Close();
        connection.Dispose();
        connection = new CsvConnection(ConnectionStrings.FolderAsDBConnectionString);
        connection.Open();

        // Act - Verify the inserted record exists in the locations table
        command = new CsvCommand($"SELECT COUNT(*) FROM locations WHERE id = {id}", connection);
        var count = (int)command.ExecuteScalar()!;

        // Assert
        Assert.True(1 == count, $"SELECTing COUNT(*) for id = {id} didn't yield 1 row.  Rows = {count}");
        connection.Close();
        connection.Dispose();
        connection = new CsvConnection(ConnectionStrings.FolderAsDBConnectionString);
        connection.Open();

        // Act - Insert a new record into the employees table
        command = new CsvCommand("INSERT INTO employees (name, email, salary, married) VALUES ('Jim Convis', 'johndoe@example.com', 100000, 'true')", connection);
        rowsAffected = command.ExecuteNonQuery();

        // Assert
        Assert.True(1 == rowsAffected, $"{nameof(command.ExecuteNonQuery)} indicated its rows affected wasn't 1.  RowsAffected = {rowsAffected}");

        // Close the connection
        connection.Close();
    }

    [Fact]
    public void Insert_JsonShouldBeFormatted()
    {
        // Arrange
        var connection = new CsvConnection(ConnectionStrings.AddFormatted(ConnectionStrings.FolderAsDBConnectionString, true));
        connection.Open();

        // Act - Insert a new record into the locations table
        const string locationsTableName = "locations";
        var command = new CsvCommand($"INSERT INTO {locationsTableName} (id, city, state, zip) VALUES (156, 'Seattle', 'Washington', 98101)", connection);
        var rowsAffected = command.ExecuteNonQuery();

        // Assert
        var jsonFileContents = File.ReadAllText(connection.GetTablePath(locationsTableName));
        Assert.Contains("\n", jsonFileContents);
    }

}
