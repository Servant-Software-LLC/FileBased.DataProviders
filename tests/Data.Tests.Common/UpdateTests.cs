using System.Data.FileClient;
using Xunit;

namespace Data.Tests.Common;

public static class UpdateTests
{
    public static void Update_ShouldUpdateData(Func<FileConnection> createFileConnection)
    {
        // Arrange
        using (var connection = createFileConnection())
        {
            var command = connection.CreateCommand();
            connection.Open();
            command.Connection = connection;

            // Insert data in both tables
            command.CommandText = "INSERT INTO employees (name,email,salary,married) values ('Majid','johndoe@email.com',50000,'true')";
            command.ExecuteNonQuery();

            command.CommandText = "INSERT INTO locations (id,city,state,zip) values (44020,'New York','NY',10001)";
            command.ExecuteNonQuery();

            command = connection.CreateCommand();

            // Act
            command.CommandText = "UPDATE employees SET salary=60000 where name='Majid'";
            int employeesUpdated = command.ExecuteNonQuery();

            command = connection.CreateCommand();

            command.CommandText = "UPDATE locations SET city='Los Angeles' where id=44020";
            int locationsUpdated = command.ExecuteNonQuery();

            // Assert
            Assert.Equal(1, employeesUpdated);
            Assert.Equal(1, locationsUpdated);
        }
    }
}
