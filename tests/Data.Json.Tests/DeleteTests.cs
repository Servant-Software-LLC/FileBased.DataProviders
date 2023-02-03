using System;
using System.Collections.Generic;
using System.Data.JsonClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Data.Json.Tests
{
    public class DeleteTests
    {
        string connectionString = "Data Source=Sources/database.json;";

        [Fact]
        public void Delete_ShouldDeleteData()
        {
         
            // Arrange Insert the data first
            using var connection = new JsonConnection(connectionString);
            var command = new JsonCommand("INSERT INTO [employees] (name, email, salary) VALUES ('JIMM', 'JIMM@gmail.com', 65000)", connection);
            //Act
            connection.Open();
            var result = command.ExecuteNonQuery();
            // Assert
            Assert.Equal(1, result);

            // Arrange Delete it
            command = new JsonCommand("DELETE FROM [employees] WHERE name='JIMM' AND salary=65000", connection);
            // Act
            result = command.ExecuteNonQuery();
            // Assert
            Assert.Equal(1, result);


            // Arrange Check it
            command = new JsonCommand("SELECT * FROM [employees] WHERE name='JIMM'", connection);
            var reader = command.ExecuteReader();

            // Assert
            Assert.False(reader.Read());
        }
    }
}
