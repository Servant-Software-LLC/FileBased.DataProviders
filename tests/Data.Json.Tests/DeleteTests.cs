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
            // Arrange
            var connection = new JsonConnection(connectionString);
            var command = new JsonCommand("DELETE FROM [employees] WHERE name='Jim'", connection);


            // Act
            connection.Open();
            var result = command.ExecuteNonQuery();
            connection.Close();

            connection = new JsonConnection(connectionString);
            connection.Open();
            // Assert
            Assert.Equal(1, result);
            var readCommand = new JsonCommand("SELECT * FROM [employees] WHERE name='Jim'", connection);
            var reader = readCommand.ExecuteReader();
            Assert.False(reader.Read());
        }
    }
}
