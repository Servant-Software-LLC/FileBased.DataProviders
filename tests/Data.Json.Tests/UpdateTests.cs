using System;
using System.Collections.Generic;
using System.Data.JsonClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Data.Json.Tests
{
    public class UpdateTests
    {
        string connectionString = "Data Source=Sources/database.json;";


        [Fact]
        public void Update_ShouldUpdateData()
        {
            // Arrange
            var connection = new JsonConnection(connectionString);
            var command = new JsonCommand("UPDATE [employees] SET salary=60000 WHERE name='Mike'", connection);


            // Act
            connection.Open();
            var result = command.ExecuteNonQuery();

            // Assert
            Assert.Equal(1, result);
            connection = new JsonConnection(connectionString);
            connection.Open();
            var readCommand = new JsonCommand("SELECT * FROM [employees] WHERE name='Mike'", connection);
            var reader = readCommand.ExecuteReader();
            while (reader.Read())
            {
                Assert.Equal(60000M, reader["salary"]);
            }
            reader.Dispose();
            connection.Close();
        }

    }
}
