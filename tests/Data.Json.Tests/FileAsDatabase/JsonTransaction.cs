using System;
using System.Collections.Generic;
using System.Data;
using System.Data.JsonClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Data.Json.Tests.FileAsDatabase
{
    public partial class JsonTransactionTests
    {
        [Fact]
        public void Transaction_ShouldInsertDataIntoDatabase()
        {
            // Arrange
            var connection = new JsonConnection(ConnectionStrings.FileAsDBConnectionString);
            connection.Open();

            // Start a transaction
            var transaction = (JsonTransaction)connection.BeginTransaction();

            // Create a command to insert data into the locations table
            var command = new JsonCommand("INSERT INTO locations (city, state) VALUES (@City, @State)", connection, transaction);

            command.Parameters.Add(new JsonParameter("City", "MiranShah"));
            command.Parameters.Add(new JsonParameter("State", "MA"));

            // Execute the command
            int rowsAffected = command.ExecuteNonQuery();



            // Create a command to insert data into the employees table
            command = new JsonCommand("INSERT INTO employees (name, salary) VALUES (@Name, @Salary)", connection, transaction);
            command.Parameters.Add(new JsonParameter("Name", "Smith Kline"));
            command.Parameters.Add(new JsonParameter("Salary", 60000));

            // Execute the command
            rowsAffected += command.ExecuteNonQuery();





            transaction.Commit();

            // Assert
            Assert.Equal(2, rowsAffected);

            // Query the locations table to verify the data was inserted
            var adapter = new JsonDataAdapter("SELECT * FROM locations WHERE city = 'MiranShah'", connection);
            var dataSet = new DataSet();
            adapter.Fill(dataSet);

            Assert.Equal(1, dataSet.Tables[0].Rows.Count);
            Assert.Equal("MiranShah", dataSet.Tables[0].Rows[0]["city"]);

            // Query the employees table to verify the data was inserted
            adapter = new JsonDataAdapter("SELECT * FROM employees WHERE name = 'Smith Kline'", connection);
            dataSet = new DataSet();
            adapter.Fill(dataSet);

            Assert.Equal(1, dataSet.Tables[0].Rows.Count);
            Assert.Equal("Smith Kline", dataSet.Tables[0].Rows[0]["name"]);

            // Close the connection
            connection.Close();
        }

    }
}
