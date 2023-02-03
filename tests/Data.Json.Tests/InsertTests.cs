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
        string folderConnectionString = "Data Source=Sources/Folder;";


        [Fact]
        public void Insert_ShouldInsertData()
        {
            // Arrange
            var connection = new JsonConnection(connectionString);
            var command = new JsonCommand("INSERT INTO [employees] (name, email, salary) VALUES ('Vincenzo', 'Vincenzo@gmail.com', 65000)", connection);
            // Act
            connection.Open();
            var result = command.ExecuteNonQuery();
            // Assert
            Assert.Equal(1, result);
            var readCommand = new JsonCommand("SELECT * FROM [employees] WHERE name='Vincenzo'", connection);
            var readResult = readCommand.ExecuteReader();
            readResult.Read();

                Assert.Equal("Vincenzo@gmail.com", readResult["email"]);
                Assert.Equal((decimal)65000, readResult["salary"]);
        }

        [Fact]
        public void FolderAsDB_Insert_ShouldInsertData()
        {
            // Arrange
            var connection = new JsonConnection(folderConnectionString);
            var command = new JsonCommand("INSERT INTO [employees] (name, email, salary) VALUES ('Vincenzo', 'Vincenzo@gmail.com', 65000)", connection);
            // Act
            connection.Open();
            var result = command.ExecuteNonQuery();
            // Assert
            Assert.Equal(1, result);

            //Arrange
            var readCommand = new JsonCommand("SELECT * FROM [employees] WHERE name='Vincenzo'", connection);
            //Act
            var readResult = readCommand.ExecuteReader();

            //Assert
            Assert.True(readResult.Read());
            Assert.Equal("Vincenzo@gmail.com", readResult["email"]);
            Assert.Equal((decimal)65000, readResult["salary"]);





            // Arrange
            command = new JsonCommand("INSERT INTO [locations] (id, city, state) VALUES (100, 'Washington', 'US')", connection);
            // Act
            connection.Open();
            result = command.ExecuteNonQuery();
            // Assert
            Assert.Equal(1, result);

            //Arrange
            readCommand = new JsonCommand("SELECT * FROM [locations] WHERE city='Washington'", connection);
            //Act
            readResult = readCommand.ExecuteReader();

            //Assert
            Assert.True(readResult.Read());
            Assert.Equal("Washington", readResult["city"]);
            Assert.Equal((decimal)100, readResult["id"]);


        }
    }
}
