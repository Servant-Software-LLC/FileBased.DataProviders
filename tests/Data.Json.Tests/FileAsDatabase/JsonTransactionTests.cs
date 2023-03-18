using System.Data;
using System.Data.JsonClient;
using Xunit;

namespace Data.Json.Tests.FileAsDatabase;

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

    [Fact]
    public void Transaction_ShouldDeleteDataFromDatabase()
    {
        // Arrange
        var connection = new JsonConnection(ConnectionStrings.FileAsDBConnectionString);
        connection.Open();

        // Start a transaction
        var transaction = (JsonTransaction)connection.BeginTransaction();

        // Insert data into the locations table
        var command = new JsonCommand("INSERT INTO locations (city, state) VALUES (@City, @State)", connection, transaction);

        command.Parameters.Add(new JsonParameter("City", "North Waziristan"));
        command.Parameters.Add(new JsonParameter("State", "MA"));

        int rowsAffected = command.ExecuteNonQuery();

        // Delete the data from the locations table
        command = new JsonCommand("DELETE FROM locations WHERE city = @City", connection, transaction);
        command.Parameters.Add(new JsonParameter("City", "North Waziristan"));

        rowsAffected += command.ExecuteNonQuery();

        transaction.Commit();

        // Assert
        Assert.Equal(1, rowsAffected);

        // Query the locations table to verify the data was deleted
        var adapter = new JsonDataAdapter("SELECT * FROM locations WHERE city = 'North Waziristan'", connection);
        var dataSet = new DataSet();
        adapter.Fill(dataSet);

        Assert.Equal(0, dataSet.Tables[0].Rows.Count);

        // Close the connection
        connection.Close();
    }

    [Fact]
    public void Transaction_ShouldUpdateDataInDatabase()
    {
        // Arrange
        var connection = new JsonConnection(ConnectionStrings.FileAsDBConnectionString);
        connection.Open();

        // Start a transaction
        var transaction = (JsonTransaction)connection.BeginTransaction();

        // Insert data into the employees table
        var command = new JsonCommand("INSERT INTO employees (name, salary) VALUES (@Name, @Salary)", connection, transaction);

        command.Parameters.Add(new JsonParameter("Name", "Shahid Khan"));
        command.Parameters.Add(new JsonParameter("Salary", 50000));

        command.ExecuteNonQuery();

        // Update the data in the employees table
        command = new JsonCommand("UPDATE employees SET salary = @Salary WHERE name = @Name", connection, transaction);
        command.Parameters.Add(new JsonParameter("Name", "Shahid Khan"));
        command.Parameters.Add(new JsonParameter("Salary", 60000));

        command.ExecuteNonQuery();

        transaction.Commit();

       

        // Query the employees table to verify the data was updated
        var adapter = new JsonDataAdapter("SELECT * FROM employees WHERE name = 'Shahid Khan'", connection);
        var dataSet = new DataSet();
        adapter.Fill(dataSet);

        Assert.Equal(1, dataSet.Tables[0].Rows.Count);
        Assert.Equal(60000M, dataSet.Tables[0].Rows[0]["salary"]);

        // Close the connection
        connection.Close();
    }

    [Fact]
    public void Transaction_ShouldRollbackWhenExceptionIsThrown()
    {
        // Arrange
        var connection = new JsonConnection(ConnectionStrings.FileAsDBConnectionString);
        connection.Open();

        // Start a transaction
        var transaction = (JsonTransaction)connection.BeginTransaction();

        try
        {
            // Create a command to insert data into the locations table
            var command = new JsonCommand("INSERT INTO locations (city, state) VALUES (@City, @State)", connection, transaction);

            command.Parameters.Add(new JsonParameter("City", "Bannu"));
            command.Parameters.Add(new JsonParameter("State", "MA"));

            // Execute the command
            int rowsAffected = command.ExecuteNonQuery();

            // Simulate an exception by dividing by zero
            int x = 0;
            int y = 1 / x;

            // If an exception is not thrown, the test will fail
            Assert.True(false, "Exception was not thrown");

            // Commit the transaction
            transaction.Commit();
        }
        catch (Exception)
        {
            // Rollback the transaction
            transaction.Rollback();

            // Query the locations table to verify that the data was not inserted
            var adapter = new JsonDataAdapter("SELECT * FROM locations WHERE city = 'Bannu'", connection);
            var dataSet = new DataSet();
            adapter.Fill(dataSet);

            Assert.Equal(0, dataSet.Tables[0].Rows.Count);
        }

        // Close the connection
        connection.Close();
    }

}
