using System.Data.JsonClient;
using Xunit;

namespace Data.Json.Tests
{
    public class UpdateTests
    {
        #region Folder as DataBase
        [Fact]
        public void Update_ShouldUpdateData()
        {
            // Arrange
            JsonConnection connection = new JsonConnection(ConnectionStrings.FolderAsDBConnectionString);
            JsonCommand command = new JsonCommand();
            connection.Open();
            command.Connection = connection;

            // Insert data in both tables
            command.CommandText = "INSERT INTO employees (name,email,salary,married) values ('John khan','johndoe@email.com',50000,'true')";
            command.ExecuteNonQuery();

            command.CommandText = "INSERT INTO locations (id,city,state,zip) values (440,'New York','NY',10001)";
            command.ExecuteNonQuery();

            // Act
            command.CommandText = "UPDATE employees SET salary=60000 where name='John khan'";
            int employeesUpdated = command.ExecuteNonQuery();

            command.CommandText = "UPDATE locations SET city='Los Angeles' where id=440";
            int locationsUpdated = command.ExecuteNonQuery();

            // Assert
            Assert.Equal(1, employeesUpdated);
            Assert.Equal(1, locationsUpdated);

            // Clean up the data from files
            command.CommandText = "DELETE FROM employees where name='John khan'";
            command.ExecuteNonQuery();

            command.CommandText = "DELETE FROM locations where id=440";
            command.ExecuteNonQuery();
        }

        #endregion

        #region File as database

        [Fact]
        public void Update_ShouldUpdateDataInFile()
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

            // Act
            command.CommandText = "UPDATE employees SET salary=60000 where name='KHAN'";
            int employeesUpdated = command.ExecuteNonQuery();

            command.CommandText = "UPDATE locations SET city='Los Angeles' where id=12310";
            int locationsUpdated = command.ExecuteNonQuery();

            // Assert
            Assert.Equal(1, employeesUpdated);
            Assert.Equal(1, locationsUpdated);

            // Clean up the data from files
            command.CommandText = "DELETE FROM employees where name='KHAN'";
            command.ExecuteNonQuery();

            command.CommandText = "DELETE FROM locations where id=12310";
            command.ExecuteNonQuery();
        }

        #endregion
    }
}
