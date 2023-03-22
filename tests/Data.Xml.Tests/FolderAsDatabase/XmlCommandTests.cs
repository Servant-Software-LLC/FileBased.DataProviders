using System.Data.XmlClient;
using Xunit;

namespace Data.Xml.Tests.FolderAsDatabase;

/// <summary>
/// Tests that exercise the <see cref="XmlCommand"/> class while using the 'Folder as Database' approach.
/// </summary>
public class XmlCommandTests
{
    [Fact]
    public void ExecuteScalar_ShouldReturnFirstRowFirstColumn()
    {
        const string query = "SELECT city, state FROM locations";

        // Arrange
        var connection = new XmlConnection(ConnectionStrings.Instance.FolderAsDBConnectionString);
        connection.Open();
        var command = new XmlCommand(query, connection);
        var reader = command.ExecuteReader();
        Assert.True(reader.Read());
        var fieldCount = reader.FieldCount;
        Assert.Equal(2, fieldCount);
        var expectedValue = reader[0];

        // Act - Per https://learn.microsoft.com/en-us/dotnet/api/system.data.idbcommand.executescalar?view=net-7.0#definition
        //       "Executes the query, and returns the first column of the first row in the resultset returned
        //       by the query. Extra columns or rows are ignored."
        var scalarCommand = new XmlCommand(query, connection);
        var value = scalarCommand.ExecuteScalar();

        // Assert
        Assert.Equal(expectedValue, value);

        // Close the connection
        connection.Close();
    }

    [Fact]
    public void ExecuteScalar_ShouldCountRecords()
    {
        // Arrange
        var connection = new XmlConnection(ConnectionStrings.Instance.FolderAsDBConnectionString);
        connection.Open();

        // Act - Count the records in the locations table
        var command = new XmlCommand("SELECT COUNT(*) FROM locations where id=1 or id=2", connection);
        var count = (int)command.ExecuteScalar()!;

        // Assert
        Assert.Equal(2, count);

        // Act - Count the records in the employees table
        command = new XmlCommand("SELECT COUNT(*) FROM employees where name='Joe' OR name='Bob' OR name='Jim' OR name='Mike'", connection);
        count = (int)command.ExecuteScalar()!;

        // Assert
        Assert.Equal(4, count);

        // Close the connection
        connection.Close();
    }

}
