using Data.Common.Extension;
using System.Data;
using System.Data.FileClient;
using Xunit;

namespace Data.Tests.Common;

public static class TransactionTests
{
    public static void Transaction_ShouldInsertDataIntoDatabase(Func<FileConnection> createFileConnection)
    {
        // Arrange
        var connection = createFileConnection();
        connection.Open();

        // Start a transaction
        var transaction = connection.BeginTransaction();

        // Create a command to insert data into the locations table
        var command = transaction.CreateCommand("INSERT INTO locations (id,city, state,zip) VALUES (@Id,@City, @State,@Zip)");

        command.Parameters.Add(command.CreateParameter("Id", "5601"));
        command.Parameters.Add(command.CreateParameter("City", "MiranShah"));
        command.Parameters.Add(command.CreateParameter("State", "MA"));
        command.Parameters.Add(command.CreateParameter("Zip", "102"));

        // Execute the command
        int rowsAffected = command.ExecuteNonQuery();


        // Create a command to insert data into the employees table
        command = transaction.CreateCommand("INSERT INTO employees (name, salary) VALUES (@Name, @Salary)");
        command.Parameters.Add(command.CreateParameter("Name", "Smith Kline"));
        command.Parameters.Add(command.CreateParameter("Salary", 60000));

        // Execute the command
        rowsAffected += command.ExecuteNonQuery();

        transaction.Commit();

        // Assert
        Assert.Equal(2, rowsAffected);

        // Query the locations table to verify the data was inserted
        var adapter = connection.CreateDataAdapter("SELECT * FROM locations WHERE city = 'MiranShah'");
        var dataSet = new DataSet();
        adapter.Fill(dataSet);

        Assert.Equal(1, dataSet.Tables[0].Rows.Count);
        Assert.Equal("MiranShah", dataSet.Tables[0].Rows[0]["city"]);

        // Query the employees table to verify the data was inserted
        adapter = connection.CreateDataAdapter("SELECT * FROM employees WHERE name = 'Smith Kline'");
        dataSet = new DataSet();
        adapter.Fill(dataSet);

        Assert.Equal(1, dataSet.Tables[0].Rows.Count);
        Assert.Equal("Smith Kline", dataSet.Tables[0].Rows[0]["name"]);

        // Close the connection
        connection.Close();

    }

    public static void Transaction_ShouldDeleteDataFromDatabase(Func<FileConnection> createFileConnection)
    {
        // Arrange
        var connection = createFileConnection();
        connection.Open();

        // Start a transaction
        var transaction = connection.BeginTransaction();

        // Insert data into the locations table
        var command = transaction.CreateCommand("INSERT INTO locations (city, state) VALUES (@City, @State)");

        command.Parameters.Add(command.CreateParameter("City", "North Waziristan"));
        command.Parameters.Add(command.CreateParameter("State", "MA"));

        int rowsAffected = command.ExecuteNonQuery();

        // Delete the data from the locations table
        command = transaction.CreateCommand("DELETE FROM locations WHERE city = @City");
        command.Parameters.Add(command.CreateParameter("City", "North Waziristan"));

        rowsAffected += command.ExecuteNonQuery();

        transaction.Commit();

        // Assert
        Assert.Equal(1, rowsAffected);

        // Query the locations table to verify the data was deleted
        var adapter = connection.CreateDataAdapter("SELECT * FROM locations WHERE city = 'North Waziristan'");
        var dataSet = new DataSet();
        adapter.Fill(dataSet);

        Assert.Equal(0, dataSet.Tables[0].Rows.Count);

        // Close the connection
        connection.Close();
    }

    public static void Transaction_ShouldUpdateDataInDatabase(Func<FileConnection> createFileConnection)
    {
        // Arrange
        var connection = createFileConnection();
        connection.Open();

        // Start a transaction
        var transaction = connection.BeginTransaction();

        // Insert data into the employees table
        var command = transaction.CreateCommand("INSERT INTO employees (name, salary) VALUES (@Name, @Salary)");

        command.Parameters.Add(command.CreateParameter("Name", "Shahid Khan"));
        command.Parameters.Add(command.CreateParameter("Salary", 10));

        command.ExecuteNonQuery();

        // Update the data in the employees table
        command = transaction.CreateCommand("UPDATE employees SET salary = @Salary WHERE name = @Name");
        command.Parameters.Add(command.CreateParameter("Name", "Shahid Khan"));
        command.Parameters.Add(command.CreateParameter("Salary", 20));

        command.ExecuteNonQuery();

        transaction.Commit();


        // Query the employees table to verify the data was updated
        var adapter = connection.CreateDataAdapter("SELECT * FROM employees WHERE name = 'Shahid Khan'");
        var dataSet = new DataSet();
        adapter.Fill(dataSet);
        var salaryOrdinal = dataSet.Tables[0].Columns.IndexOf("salary");
        Assert.Equal(1, dataSet.Tables[0].Rows.Count);
        Assert.Equal(20M, dataSet.Tables[0].Rows[0][salaryOrdinal].GetValueAsType<decimal>());

        // Close the connection
        connection.Close();

    }
}
