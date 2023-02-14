using System.Data.JsonClient;
using Xunit;

namespace Data.Json.Tests.FileAsDatabase;

public class JsonUpdateTests
{
    [Fact]
    public void Update_ShouldUpdateData()
    {
        // Arrange
        JsonConnection connection = new JsonConnection(ConnectionStrings.FileAsDBConnectionString);
        JsonCommand command = new JsonCommand();
        connection.Open();
        command.Connection = connection;

        // Insert data in both tables
        command.CommandText = "INSERT INTO employees (name,email,salary,married) values ('KHAN','johndoe@email.com',50000,'true')";
        command.ExecuteNonQuery();

        command.CommandText = "INSERT INTO locations (id,city,state,zip) values (12310,'New York','NY',10001)";
        command.ExecuteNonQuery();

        connection = new JsonConnection(ConnectionStrings.FileAsDBConnectionString);
        connection.Open();

        // Act
        command.CommandText = "UPDATE employees SET salary=60000 where name='KHAN'";
        int employeesUpdated = command.ExecuteNonQuery();
        connection = new JsonConnection(ConnectionStrings.FileAsDBConnectionString);
        connection.Open();
        command.CommandText = "UPDATE locations SET city='Los Angeles' where id=12310";
        int locationsUpdated = command.ExecuteNonQuery();

        // Assert
        Assert.Equal(1, employeesUpdated);
        Assert.Equal(1, locationsUpdated);
    }

}
