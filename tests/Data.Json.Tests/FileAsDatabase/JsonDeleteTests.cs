using System.Data.JsonClient;
using Xunit;

namespace Data.Json.Tests.FileAsDatabase;

public class JsonDeleteTests
{
    [Fact]
    public void Delete_ShouldDeleteData()
    {
        // Arrange
        JsonConnection connection = new JsonConnection(ConnectionStrings.FileAsDBConnectionString);
        JsonCommand command = new JsonCommand();
        command.Connection = connection;
        connection.Open();
        // Insert data in both tables
        command.CommandText = "INSERT INTO employees (name,email,salary,married) values ('Malizba File','johndoe@email.com',50000,'true')";
        command.ExecuteNonQuery();

        command.CommandText = "INSERT INTO locations (id,city,state,zip) values (12250,'New York','NY',10001)";
        command.ExecuteNonQuery();

        connection = new JsonConnection(ConnectionStrings.FileAsDBConnectionString);
        connection.Open();

        // Act
        command.CommandText = "DELETE FROM employees where name='Malizba File'";
        int employeesDeleted = command.ExecuteNonQuery();


        connection = new JsonConnection(ConnectionStrings.FileAsDBConnectionString);
        connection.Open();

        command.CommandText = "DELETE FROM locations where id=12250";
        int locationsDeleted = command.ExecuteNonQuery();

        // Assert
        Assert.Equal(1, employeesDeleted);
        Assert.Equal(1, locationsDeleted);
    }

}
