using System.Data.JsonClient;
using Xunit;

namespace Data.Json.Tests.FolderAsDb
{
    public class UpdateTests
    {
        [Fact]
        public void Update_ShouldUpdateDataInBothTables()
        {
            // Arrange
            JsonConnection connection = new JsonConnection("Data Source=C:\\Users\\GAC\\source\\repos\\ADO.NET.FileBased.DataProviders\\tests\\Data.Json.Tests\\Sources\\Folder");
            JsonCommand command = new JsonCommand();
            connection.Open();
            command.Connection = connection;

            // Insert data in both tables
            command.CommandText = "INSERT INTO employees (name,email,salary,married) values ('John Doe','johndoe@email.com',50000,'true')";
            command.ExecuteNonQuery();

            command.CommandText = "INSERT INTO locations (id,city,state,zip) values (10,'New York','NY',10001)";
            command.ExecuteNonQuery();

            // Act
            command.CommandText = "UPDATE employees SET salary=60000 where name='John Doe'";
            int employeesUpdated = command.ExecuteNonQuery();

            command.CommandText = "UPDATE locations SET city='Los Angeles' where id=10";
            int locationsUpdated = command.ExecuteNonQuery();

            // Assert
            Assert.Equal(1, employeesUpdated);
            Assert.Equal(1, locationsUpdated);

            // Clean up the data from files
            command.CommandText = "DELETE FROM employees where name='John Doe'";
            command.ExecuteNonQuery();

            command.CommandText = "DELETE FROM locations where id=1";
            command.ExecuteNonQuery();
        }

    }
}
