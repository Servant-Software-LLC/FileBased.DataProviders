using System.Data.XmlClient;
using Xunit;

namespace Data.Xml.Tests.FolderAsDatabase;

/// <summary>
/// These tests indirectly exercise the <see cref="JsonIO.Write.JsonUpdate"/> class via calls to <see cref="XmlCommand.ExecuteNonQuery" />/>. 
/// </summary>
public class XmlnUpdateTests
{
    [Fact]
    public void Update_ShouldUpdateData()
    {
        // Arrange
        XmlConnection connection = new XmlConnection(ConnectionStrings.FolderAsDBConnectionString);
        XmlCommand command = new XmlCommand();
        connection.Open();
        command.Connection = connection;

        // Insert data in both tables
        command.CommandText = "INSERT INTO employees (name,email,salary,married) values ('Majid','johndoe@email.com',50000,'true')";
        command.ExecuteNonQuery();

        command.CommandText = "INSERT INTO locations (id,city,state,zip) values (44020,'New York','NY',10001)";
        command.ExecuteNonQuery();


        connection = new XmlConnection(ConnectionStrings.FolderAsDBConnectionString);
        connection.Open();

        // Act
        command.CommandText = "UPDATE employees SET salary=60000 where name='Majid'";
        int employeesUpdated = command.ExecuteNonQuery();

        connection = new XmlConnection(ConnectionStrings.FolderAsDBConnectionString);
        connection.Open();

        command.CommandText = "UPDATE locations SET city='Los Angeles' where id=44020";
        int locationsUpdated = command.ExecuteNonQuery();

        // Assert
        Assert.Equal(1, employeesUpdated);
        Assert.Equal(1, locationsUpdated);
    }

}
