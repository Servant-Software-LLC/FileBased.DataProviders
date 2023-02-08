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
        #region Folder as database
        [Fact]
        public void Insert_ShouldInsertData()
        {
            // Arrange

            var connection = new JsonConnection(ConnectionStrings.FolderAsDBConnectionString);
            connection.Open();

            // Act - Insert a new record into the locations table
            var command = new JsonCommand("INSERT INTO locations (id, city, state, zip) VALUES (100, 'Seattle', 'Washington', 98101)", connection);
            var rowsAffected = command.ExecuteNonQuery();

            // Assert
            Assert.Equal(1, rowsAffected);

            // Act - Verify the inserted record exists in the locations table
            command = new JsonCommand("SELECT COUNT(*) FROM locations WHERE id = 100", connection);
            var count = (int)command.ExecuteScalar();

            // Assert
            Assert.Equal(1, count);

            // Act - Insert a new record into the employees table
            command = new JsonCommand("INSERT INTO employees (name, email, salary, married) VALUES ('Jim Convis', 'johndoe@example.com', 100000, 'true')", connection);
            rowsAffected = command.ExecuteNonQuery();

            // Assert
            Assert.Equal(1, rowsAffected);

            // Act - Verify the inserted record exists in the employees table
            command = new JsonCommand("SELECT COUNT(*) FROM employees WHERE name = 'Jim Convis'", connection);
            count = (int)command.ExecuteScalar();

            // Assert
            Assert.Equal(1, count);

            // Cleanup - Delete the inserted records so they don't affect other test cases
            command = new JsonCommand("DELETE FROM locations WHERE id = 100", connection);
            command.ExecuteNonQuery();

            command = new JsonCommand("DELETE FROM employees WHERE name = 'Jim Convis'", connection);
            command.ExecuteNonQuery();

            // Close the connection
            connection.Close();
        }
        #endregion

        #region File as database

        [Fact]
        public void Insert_ShouldInsertDataIntoFile()
        {
            // Arrange

            var connection = new JsonConnection(ConnectionStrings.FolderAsDBConnectionString);
            connection.Open();

            // Act - Insert a new record into the locations table
            var command = new JsonCommand("INSERT INTO locations (id, city, state, zip) VALUES (156, 'Seattle', 'Washington', 98101)", connection);
            var rowsAffected = command.ExecuteNonQuery();

            // Assert
            Assert.Equal(1, rowsAffected);

            // Act - Verify the inserted record exists in the locations table
            command = new JsonCommand("SELECT COUNT(*) FROM locations WHERE id = 156", connection);
            var count = (int)command.ExecuteScalar();

            // Assert
            Assert.Equal(1, count);

            // Act - Insert a new record into the employees table
            command = new JsonCommand("INSERT INTO employees (name, email, salary, married) VALUES ('Haleem', 'johndoe@example.com', 100000, 'true')", connection);
            rowsAffected = command.ExecuteNonQuery();

            // Assert
            Assert.Equal(1, rowsAffected);

            // Act - Verify the inserted record exists in the employees table
            command = new JsonCommand("SELECT COUNT(*) FROM employees WHERE name = 'Haleem'", connection);
            count = (int)command.ExecuteScalar();

            // Assert
            Assert.Equal(1, count);

            // Cleanup - Delete the inserted records so they don't affect other test cases
            command = new JsonCommand("DELETE FROM locations WHERE id = 156", connection);
            command.ExecuteNonQuery();

            command = new JsonCommand("DELETE FROM employees WHERE name = 'Haleem'", connection);
            command.ExecuteNonQuery();

            // Close the connection
            connection.Close();
        }
        #endregion
    }
}
