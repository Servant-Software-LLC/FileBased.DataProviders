using System;
using System.Collections.Generic;
using System.Data.JsonClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Data.Json.Tests
{
    public class ReaderTests
    {
        #region Folder as database
        [Fact]
        public void Reader_ShouldReadData()
        {
            // Arrange
            var connection = new JsonConnection(ConnectionStrings.FolderAsDBConnectionString);
            connection.Open();

            // Act - Query the locations table
            var command = new JsonCommand("SELECT * FROM locations", connection);
            var reader = command.ExecuteReader();

            // Assert
            Assert.True(reader.Read());
            var fieldCount = reader.FieldCount;
            Assert.Equal(4, fieldCount);

            // Act - Query the employees table
            command = new JsonCommand("SELECT * FROM employees", connection);
            reader = command.ExecuteReader();

            // Assert
            Assert.True(reader.Read());
            fieldCount = reader.FieldCount;
            Assert.Equal(4, fieldCount);

            // Close the connection
            connection.Close();
        }



        [Fact]
        public void Reader_ShouldReadDataWithInnerJoin()
        {
            // Arrange
            var connection = new JsonConnection(ConnectionStrings.FolderAsDBConnectionString);
            connection.Open();

            // Act
            var command = new JsonCommand("SELECT [l].[id], [l].[city], [l].[state], [l].[zip], [e].[name], [e].[email], [e].[salary], [e].[married] FROM [locations l] INNER JOIN [employees e] ON [l].[id] = [e].[salary];", connection);


            var reader = command.ExecuteReader();

            // Assert
            Assert.False(reader.Read());
            var fieldCount = reader.FieldCount;
            Assert.Equal(8, fieldCount);

            // Close the connection
            connection.Close();
        }
        [Fact]
        public void Reader_ShouldReadDataWithSelectedColumns()
        {
            // Arrange
            var connection = new JsonConnection(ConnectionStrings.FolderAsDBConnectionString);
            connection.Open();

            // Act - Query two columns from the locations table
            var command = new JsonCommand("SELECT city, state FROM locations", connection);
            var reader = command.ExecuteReader();

            // Assert
            Assert.True(reader.Read());
            var fieldCount = reader.FieldCount;
            Assert.Equal(2, fieldCount);

            // Act - Query two columns from the employees table
            command = new JsonCommand("SELECT name, salary FROM employees", connection);
            reader = command.ExecuteReader();

            // Assert
            Assert.True(reader.Read());
            fieldCount = reader.FieldCount;
            Assert.Equal(2, fieldCount);

            // Close the connection
            connection.Close();
        }
        [Fact]
        public void Reader_ShouldCountRecords()
        {
            // Arrange
            var connection = new JsonConnection(ConnectionStrings.FolderAsDBConnectionString);
            connection.Open();

            // Act - Count the records in the locations table
            var command = new JsonCommand("SELECT COUNT(*) FROM locations where id=1 or id=2", connection);
            var count = (int)command.ExecuteScalar();

            // Assert
            Assert.Equal(2, count);

            // Act - Count the records in the employees table
            command = new JsonCommand("SELECT COUNT(*) FROM employees where name='Joe' OR name='Bob' OR name='Jim' OR name='Mike'", connection);
            count = (int)command.ExecuteScalar();

            // Assert
            Assert.Equal(4, count);

            // Close the connection
            connection.Close();
        }
        #endregion

        #region File as database
        [Fact]
        public void Reader_ShouldReadDataFromFile()
        {
            // Arrange
            var connection = new JsonConnection(ConnectionStrings.FolderAsDBConnectionString);
            connection.Open();

            // Act - Query the locations table
            var command = new JsonCommand("SELECT * FROM locations", connection);
            var reader = command.ExecuteReader();

            // Assert
            Assert.True(reader.Read());
            var fieldCount = reader.FieldCount;
            Assert.Equal(4, fieldCount);

            // Act - Query the employees table
            command = new JsonCommand("SELECT * FROM employees", connection);
            reader = command.ExecuteReader();

            // Assert
            Assert.True(reader.Read());
            fieldCount = reader.FieldCount;
            Assert.Equal(4, fieldCount);

            // Close the connection
            connection.Close();
        }



        [Fact]
        public void Reader_ShouldReadDataWithInnerJoinFromFile()
        {
            // Arrange
            var connection = new JsonConnection(ConnectionStrings.FolderAsDBConnectionString);
            connection.Open();

            // Act
            var command = new JsonCommand("SELECT [l].[id], [l].[city], [l].[state], [l].[zip], [e].[name], [e].[email], [e].[salary], [e].[married] FROM [locations l] INNER JOIN [employees e] ON [l].[id] = [e].[salary];", connection);
            var reader = command.ExecuteReader();

            // Assert
            Assert.False(reader.Read());
            var fieldCount = reader.FieldCount;
            Assert.Equal(8, fieldCount);

            // Close the connection
            connection.Close();
        }
        [Fact]
        public void Reader_ShouldReadDataWithSelectedColumnsFromFile()
        {
            // Arrange
            var connection = new JsonConnection(ConnectionStrings.FolderAsDBConnectionString);
            connection.Open();

            // Act - Query two columns from the locations table
            var command = new JsonCommand("SELECT city, state FROM locations", connection);
            var reader = command.ExecuteReader();

            // Assert
            Assert.True(reader.Read());
            var fieldCount = reader.FieldCount;
            Assert.Equal(2, fieldCount);

            // Act - Query two columns from the employees table
            command = new JsonCommand("SELECT name, salary FROM employees", connection);
            reader = command.ExecuteReader();

            // Assert
            Assert.True(reader.Read());
            fieldCount = reader.FieldCount;
            Assert.Equal(2, fieldCount);

            // Close the connection
            connection.Close();
        }
        [Fact]
        public void Reader_ShouldCountRecordsFromFile()
        {
            // Arrange
            var connection = new JsonConnection(ConnectionStrings.FolderAsDBConnectionString);
            connection.Open();

            // Act - Count the records in the locations table
            var command = new JsonCommand("SELECT COUNT(*) FROM locations where id=1 or id=2", connection);

            var count = (int)command.ExecuteScalar();

            // Assert
            Assert.Equal(2, count);

            // Act - Count the records in the employees table
            command = new JsonCommand("SELECT COUNT(*) FROM employees where name='Joe' OR name='Bob' OR name='Jim' OR name='Mike'", connection);
            count = (int)command.ExecuteScalar();

            // Assert
            Assert.Equal(4, count);

            // Close the connection
            connection.Close();
        }
        #endregion


    }
}
