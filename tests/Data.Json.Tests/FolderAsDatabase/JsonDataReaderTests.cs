using System.Data.JsonClient;
using Xunit;

namespace Data.Json.Tests.FolderAsDatabase;

/// <summary>
/// Tests that exercise the <see cref="JsonDataReader"/> class.
/// </summary>
public class JsonDataReaderTests
{
    [Fact]
    public void Reader_ShouldReadData()
    {
        // Arrange
        var connection = new JsonConnection(ConnectionStrings.Instance.FolderAsDBConnectionString);
        connection.Open();

        // Act - Query the locations table
        var command = new JsonCommand("SELECT * FROM locations", connection);
        var reader = command.ExecuteReader();

        // Assert
        Assert.True(reader.Read());
        var fieldCount = reader.FieldCount;
        Assert.Equal(4, fieldCount);

        // Act - Query the employees table
        command = new JsonCommand("SELECT * FROM employees", connection);
        reader = command.ExecuteReader();

        // Assert
        Assert.True(reader.Read());
        fieldCount = reader.FieldCount;
        Assert.Equal(4, fieldCount);

        // Close the connection
        connection.Close();
    }

    [Fact]
    public void Reader_ShouldReturnData()
    {
        // Arrange
        var connection = new JsonConnection(ConnectionStrings.Instance.FolderAsDBConnectionString);
        var command = new JsonCommand("SELECT * FROM [employees]", connection);

        // Act
        connection.Open();
        using (var reader = command.ExecuteReader())
        {
            // Assert
            Assert.NotNull(reader);
            Assert.Equal(4, reader.FieldCount);

            //first Row
            Assert.True(reader.Read());
            Assert.Equal("Joe", reader["name"]);
            Assert.IsType<string>(reader["name"]);
            Assert.Equal("Joe@gmail.com", reader["email"]);
            Assert.IsType<string>(reader["email"]);
            Assert.Equal(56000M, reader["salary"]);
            Assert.IsType<decimal>(reader["salary"]);
            Assert.Equal(true, reader["married"]);
            Assert.IsType<bool>(reader["married"]);

            //second row
            Assert.True(reader.Read());
            Assert.Equal("Bob", reader["name"]);
            Assert.IsType<string>(reader["name"]);
            Assert.Equal("bob32@gmail.com", reader["email"]);
            Assert.IsType<string>(reader["email"]);
            Assert.Equal((decimal)95000, reader["salary"]);
            Assert.IsType<decimal>(reader["salary"]);
            Assert.Equal(DBNull.Value, reader["married"]);
            //this will be dbnull not bool?
            Assert.IsType<DBNull>(reader["married"]);
        }

        connection.Close();
    }

    [Fact]
    public void Reader_ShouldReturnData_WithFilter()
    {
        // Arrange
        var connection = new JsonConnection(ConnectionStrings.Instance.FolderAsDBConnectionString);
        var command = new JsonCommand("SELECT * FROM [locations] WHERE zip = 78132", connection);

        // Act
        connection.Open();
        using (var reader = command.ExecuteReader())
        {
            // Assert
            Assert.NotNull(reader);
            Assert.Equal(4, reader.FieldCount);

            //first Row
            Assert.True(reader.Read());
            Assert.Equal("New Braunfels", reader["city"]);

            //No second row
            Assert.False(reader.Read());
        }

        connection.Close();
    }

    [Fact]
    public void Reader_ShouldReadDataWithInnerJoin()
    {
        // Arrange
        var connection = new JsonConnection(ConnectionStrings.Instance.FolderAsDBConnectionString);
        connection.Open();

        // Act
        var command = new JsonCommand("SELECT [l].[id], [l].[city], [l].[state], [l].[zip], [e].[name], [e].[email], [e].[salary], [e].[married] FROM [locations l] INNER JOIN [employees e] ON [l].[id] = [e].[salary];", connection);


        var reader = command.ExecuteReader();

        // Assert
        Assert.False(reader.Read());
        var fieldCount = reader.FieldCount;
        Assert.Equal(8, fieldCount);

        // Close the connection
        connection.Close();
    }

    [Fact]
    public void Reader_ShouldReadDataWithSelectedColumns()
    {
        // Arrange
        var connection = new JsonConnection(ConnectionStrings.Instance.FolderAsDBConnectionString);
        connection.Open();

        // Act - Query two columns from the locations table
        var command = new JsonCommand("SELECT city, state FROM locations", connection);
        var reader = command.ExecuteReader();

        // Assert
        Assert.True(reader.Read());
        var fieldCount = reader.FieldCount;
        Assert.Equal(2, fieldCount);

        // Act - Query two columns from the employees table
        command = new JsonCommand("SELECT name, salary FROM employees", connection);
        reader = command.ExecuteReader();

        // Assert
        Assert.True(reader.Read());
        fieldCount = reader.FieldCount;
        Assert.Equal(2, fieldCount);

        // Close the connection
        connection.Close();
    }

}
