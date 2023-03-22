using System.Data.CsvClient;
using Xunit;

namespace Data.Csv.Tests.FolderAsDatabase;

/// <summary>
/// Tests that exercise the <see cref="CsvDataReader"/> class.
/// </summary>
public class CsvDataReaderTests
{
    [Fact]
    public void Reader_ShouldReadData()
    {
        // Arrange
        var connection = new CsvConnection(ConnectionStrings.Instance.FolderAsDBConnectionString);
        connection.Open();

        // Act - Query the locations table
        var command = new CsvCommand("SELECT * FROM locations", connection);
        var reader = command.ExecuteReader();

        // Assert
        Assert.True(reader.Read());
        var fieldCount = reader.FieldCount;
        Assert.Equal(4, fieldCount);

        // Act - Query the employees table
        command = new CsvCommand("SELECT * FROM employees", connection);
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
        var connection = new CsvConnection(ConnectionStrings.Instance.FolderAsDBConnectionString);
        var command = new CsvCommand("SELECT * FROM [employees]", connection);

        // Act
        connection.Open();
        using (var reader = command.ExecuteReader())
        {
            // Assert
            Assert.NotNull(reader);
            Assert.Equal(4, reader.FieldCount);
            var salaryOrdinal = reader.GetOrdinal("salary");
            var marriedOrdinal = reader.GetOrdinal("married");
            //first Row
            Assert.True(reader.Read());
            Assert.Equal("Joe", reader["name"]);
            Assert.IsType<string>(reader["name"]);
            Assert.Equal("Joe@gmail.com", reader["email"]);
            Assert.IsType<string>(reader["email"]);
            Assert.Equal(56000M, reader.GetDecimal(salaryOrdinal));
            Assert.IsType<decimal>(reader.GetDecimal(salaryOrdinal));
            Assert.True(reader.GetBoolean(marriedOrdinal));
            Assert.IsType<bool>(reader.GetBoolean(marriedOrdinal));

            //second row
            Assert.True(reader.Read());
            Assert.Equal("Bob", reader["name"]);
            Assert.IsType<string>(reader["name"]);
            Assert.Equal("bob32@gmail.com", reader["email"]);
            Assert.IsType<string>(reader["email"]);
            Assert.Equal(95000M, reader.GetDecimal(salaryOrdinal));
            Assert.IsType<decimal>(reader.GetDecimal(salaryOrdinal));
            Assert.Equal(string.Empty, reader["married"]);
        }

        connection.Close();
    }

    [Fact]
    public void Reader_ShouldReturnData_WithFilter()
    {
        // Arrange
        var connection = new CsvConnection(ConnectionStrings.Instance.FolderAsDBConnectionString);
        var command = new CsvCommand("SELECT * FROM [locations] WHERE zip = 78132", connection);

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
        var connection = new CsvConnection(ConnectionStrings.Instance.FolderAsDBConnectionString);
        connection.Open();

        // Act
        var command = new CsvCommand("SELECT [l].[id], [l].[city], [l].[state], [l].[zip], [e].[name], [e].[email], [e].[salary], [e].[married] FROM [locations l] INNER JOIN [employees e] ON [l].[id] = [e].[salary];", connection);


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
        var connection = new CsvConnection(ConnectionStrings.Instance.FolderAsDBConnectionString);
        connection.Open();

        // Act - Query two columns from the locations table
        var command = new CsvCommand("SELECT city, state FROM locations", connection);
        var reader = command.ExecuteReader();

        // Assert
        Assert.True(reader.Read());
        var fieldCount = reader.FieldCount;
        Assert.Equal(2, fieldCount);

        // Act - Query two columns from the employees table
        command = new CsvCommand("SELECT name, salary FROM employees", connection);
        reader = command.ExecuteReader();

        // Assert
        Assert.True(reader.Read());
        fieldCount = reader.FieldCount;
        Assert.Equal(2, fieldCount);

        // Close the connection
        connection.Close();
    }

}
