using System.Data.FileClient;
using Xunit;

namespace Data.Csv.Tests.FolderAsDatabase;

/// <summary>
/// Tests that exercise the <see cref="FileCommand"/> class while using the 'Folder as Database' approach.
/// </summary>
public static class CommandTests
{
    public static void ExecuteScalar_ShouldReturnFirstRowFirstColumn(Func<FileConnection> createConnection)
    {
        const string query = "SELECT city, state FROM locations";

        // Arrange
        var connection = createConnection();
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        var reader = command.ExecuteReader();
        Assert.True(reader.Read());
        var fieldCount = reader.FieldCount;
        Assert.Equal(2, fieldCount);
        var expectedValue = reader[0];

        // Act - Per https://learn.microsoft.com/en-us/dotnet/api/system.data.idbcommand.executescalar?view=net-7.0#definition
        //       "Executes the query, and returns the first column of the first row in the resultset returned
        //       by the query. Extra columns or rows are ignored."
        var scalarCommand = connection.CreateCommand();
        scalarCommand.CommandText = query;
        var value = scalarCommand.ExecuteScalar();

        // Assert
        Assert.Equal(expectedValue, value);

        // Close the connection
        connection.Close();
    }

    public static void ExecuteScalar_ShouldCountRecords(Func<FileConnection> createConnection)
    {
        // Arrange
        var connection = createConnection();
        connection.Open();

        // Act - Count the records in the locations table
        var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM locations where id=1 or id=2";

        var count = (int)command.ExecuteScalar()!;

        // Assert
        Assert.Equal(2, count);

        // Act - Count the records in the employees table
        command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM employees where name='Joe' OR name='Bob' OR name='Jim' OR name='Mike'";
        count = (int)command.ExecuteScalar()!;

        // Assert
        Assert.Equal(4, count);

        // Close the connection
        connection.Close();
    }
}