using Data.Common.Extension;
using System.Data;
using System.Data.XmlClient;
using Xunit;

namespace Data.Xml.Tests.FolderAsDatabase;

public partial class CsvTransactionTests
{
    [Fact]
    public void Transaction_ShouldInsertDataIntoDatabase()
    {
        // Arrange
        var connection = new CsvConnection(ConnectionStrings.FolderAsDBConnectionString);
        connection.Open();

        // Start a transaction
        var transaction = (CsvTransaction)connection.BeginTransaction();

        // Create a command to insert data into the locations table
        var command = new CsvCommand("INSERT INTO locations (id,city, state,zip) VALUES (@Id,@City, @State,@Zip)", connection, transaction);

        command.Parameters.Add(new CsvParameter("Id", "5601"));
        command.Parameters.Add(new CsvParameter("City", "MiranShah"));
        command.Parameters.Add(new CsvParameter("State", "MA"));
        command.Parameters.Add(new CsvParameter("Zip", "102"));

        // Execute the command
        int rowsAffected = command.ExecuteNonQuery();


        // Create a command to insert data into the employees table
        command = new CsvCommand("INSERT INTO employees (name, salary) VALUES (@Name, @Salary)", connection, transaction);
        command.Parameters.Add(new CsvParameter("Name", "Smith Kline"));
        command.Parameters.Add(new CsvParameter("Salary", 60000));

        // Execute the command
        rowsAffected += command.ExecuteNonQuery();

        transaction.Commit();

        // Assert
        Assert.Equal(2, rowsAffected);

        // Query the locations table to verify the data was inserted
        var adapter = new CsvDataAdapter("SELECT * FROM locations WHERE city = 'MiranShah'", connection);
        var dataSet = new DataSet();
        adapter.Fill(dataSet);

        Assert.Equal(1, dataSet.Tables[0].Rows.Count);
        Assert.Equal("MiranShah", dataSet.Tables[0].Rows[0]["city"]);

        // Query the employees table to verify the data was inserted
        adapter = new CsvDataAdapter("SELECT * FROM employees WHERE name = 'Smith Kline'", connection);
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
        var connection = new CsvConnection(ConnectionStrings.FolderAsDBConnectionString);
        connection.Open();

        // Start a transaction
        var transaction = (CsvTransaction)connection.BeginTransaction();

        // Insert data into the locations table
        var command = new CsvCommand("INSERT INTO locations (city, state) VALUES (@City, @State)", connection, transaction);

        command.Parameters.Add(new CsvParameter("City", "North Waziristan"));
        command.Parameters.Add(new CsvParameter("State", "MA"));

        int rowsAffected = command.ExecuteNonQuery();

        // Delete the data from the locations table
        command = new CsvCommand("DELETE FROM locations WHERE city = @City", connection, transaction);
        command.Parameters.Add(new CsvParameter("City", "North Waziristan"));

        rowsAffected += command.ExecuteNonQuery();

        transaction.Commit();

        // Assert
        Assert.Equal(1, rowsAffected);

        // Query the locations table to verify the data was deleted
        var adapter = new CsvDataAdapter("SELECT * FROM locations WHERE city = 'North Waziristan'", connection);
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
        var connection = new CsvConnection(ConnectionStrings.FolderAsDBConnectionString);
        connection.Open();

        // Start a transaction
        var transaction = (CsvTransaction)connection.BeginTransaction();

        // Insert data into the employees table
        var command = new CsvCommand("INSERT INTO employees (name, salary) VALUES (@Name, @Salary)", connection, transaction);

        command.Parameters.Add(new CsvParameter("Name", "Shahid Khan"));
        command.Parameters.Add(new CsvParameter("Salary", 10));

        command.ExecuteNonQuery();

        // Update the data in the employees table
        command = new CsvCommand("UPDATE employees SET salary = @Salary WHERE name = @Name", connection, transaction);
        command.Parameters.Add(new CsvParameter("Name", "Shahid Khan"));
        command.Parameters.Add(new CsvParameter("Salary", 20));

        command.ExecuteNonQuery();

        transaction.Commit();


        // Query the employees table to verify the data was updated
        var adapter = new CsvDataAdapter("SELECT * FROM employees WHERE name = 'Shahid Khan'", connection);
        var dataSet = new DataSet();
        adapter.Fill(dataSet);
        var salaryOrdinal = dataSet.Tables[0].Columns.IndexOf("salary");
        Assert.Equal(1, dataSet.Tables[0].Rows.Count);
        Assert.Equal(20M, dataSet.Tables[0].Rows[0][salaryOrdinal].GetValueAsType<decimal>());

        // Close the connection
        connection.Close();
    }

    [Fact]
    public void Transaction_ShouldRollbackWhenExceptionIsThrown()
    {
        // Arrange
        var connection = new CsvConnection(ConnectionStrings.FolderAsDBConnectionString);
        connection.Open();

        // Start a transaction
        var transaction = (CsvTransaction)connection.BeginTransaction();

        try
        {
            // Create a command to insert data into the locations table
            var command = new CsvCommand("INSERT INTO locations (city, state) VALUES (@City, @State)", connection, transaction);

            command.Parameters.Add(new CsvParameter("City", "Bannu"));
            command.Parameters.Add(new CsvParameter("State", "MA"));

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
            var adapter = new CsvDataAdapter("SELECT * FROM locations WHERE city = 'Bannu'", connection);
            var dataSet = new DataSet();
            adapter.Fill(dataSet);

            Assert.Equal(0, dataSet.Tables[0].Rows.Count);
        }

        // Close the connection
        connection.Close();
    }

}
