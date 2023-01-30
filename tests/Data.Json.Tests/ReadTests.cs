using Iced.Intel;
using System;
using System.Collections.Generic;
using System.Data.JsonClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Data.Json.Tests
{
    public class ReadTests
    {
        
        string connectionString = "Data Source=Sources/database.json;";
        [Fact]
        public void Read_ShouldReturnData()
        {
            // Arrange
            var connection = new JsonConnection(connectionString);
            var command = new JsonCommand("SELECT * FROM [employees]", connection);
            // Act
            connection.Open();
            var reader = command.ExecuteReader();

            // Assert
            Assert.NotNull(reader);
            Assert.Equal(4, reader.FieldCount);
            //first Row
            Assert.True(reader.Read());
            Assert.Equal("Joe", reader["name"]);
            Assert.IsType<string>(reader["name"]);
            Assert.Equal("Joe@gmail.com", reader["email"]);
            Assert.IsType<string>(reader["email"]);
            Assert.Equal(56000M, reader["salary"]);
            Assert.IsType<decimal>(reader["salary"]);
            Assert.Equal(true, reader["married"]);
            Assert.IsType<bool>(reader["married"]);

            //second row
            Assert.True(reader.Read());
            Assert.Equal("Bob", reader["name"]);
            Assert.IsType<string>(reader["name"]);
            Assert.Equal("bob32@gmail.com", reader["email"]);
            Assert.IsType<string>(reader["email"]);
            Assert.Equal((decimal)95000, reader["salary"]);
            Assert.IsType<decimal>(reader["salary"]);
            Assert.Equal(DBNull.Value, reader["married"]);
            //this will be dbnull not bool?
            Assert.IsType<DBNull>(reader["married"]);

            reader.Dispose();
            connection.Close();
        }


  

   
  
    }
}
