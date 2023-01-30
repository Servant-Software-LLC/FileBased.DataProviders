using System;
using System.Collections.Generic;
using System.Data.JsonClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Data.Json.Tests
{
    public class InsertTests
    {
        string connectionString = "Data Source=Sources/database.json;";


        [Fact]
        public void Insert_ShouldInsertData()
        {
            // Arrange
            var connection = new JsonConnection(connectionString);
            var command = new JsonCommand("INSERT INTO [employees] (name, email, salary) VALUES ('Jane', 'jane@gmail.com', 65000)", connection);


            // Act
            connection.Open();
            var result = command.ExecuteNonQuery();
            //Arrange
            connection = new JsonConnection(connectionString);
            connection.Open();

            // Assert
            Assert.Equal(1, result);
            var readCommand = new JsonCommand("SELECT * FROM [employees] WHERE name='Jane'", connection);
            var readResult = readCommand.ExecuteReader();
            while (readResult.Read())
            {
                Assert.Equal("jane@gmail.com", readResult["email"]);
                Assert.Equal((decimal)65000, readResult["salary"]);
            }
        }
    }
}
