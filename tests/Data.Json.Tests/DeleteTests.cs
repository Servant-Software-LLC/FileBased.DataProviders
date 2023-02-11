using System.Data.JsonClient;
using Xunit;

namespace Data.Json.Tests;


public class DeleteTests
{
    #region Folder as database
    [Fact]
    public void Delete_ShouldDeleteData()
    {
        // Arrange
        JsonConnection connection = new JsonConnection(ConnectionStrings.FolderAsDBConnectionString);
        JsonCommand command = new JsonCommand();
        command.Connection = connection;
        connection.Open();
        // Insert data in both tables
        command.CommandText = "INSERT INTO employees (name,email,salary,married) values ('Malizba','johndoe@email.com',50000,'true')";
        command.ExecuteNonQuery();

        command.CommandText = "INSERT INTO locations (id,city,state,zip) values (1350,'New York','NY',10001)";
        command.ExecuteNonQuery();
        connection = new JsonConnection(ConnectionStrings.FolderAsDBConnectionString);
        connection.Open();
        // Act
        command.CommandText = "DELETE FROM employees where name='Malizba'";
        int employeesDeleted = command.ExecuteNonQuery();
        
        connection = new JsonConnection(ConnectionStrings.FolderAsDBConnectionString);
        connection.Open();
        command.CommandText = "DELETE FROM locations where id=1350";
        int locationsDeleted = command.ExecuteNonQuery();

        // Assert
        Assert.Equal(1, employeesDeleted);
        Assert.Equal(1, locationsDeleted);
    }
    #endregion

    #region File as database
    [Fact]
    public void Delete_ShouldDeleteDataFromFile()
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
    #endregion
}
