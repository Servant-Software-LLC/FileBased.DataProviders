using Data.Common.Extension;
using System.Data;
using System.Data.FileClient;
using Xunit;

namespace Data.Tests.Common;

public static class TransactionTests
{
    public static void Transaction_ShouldInsertDataIntoDatabase<TFileParameter>(Func<FileConnection<TFileParameter>> createFileConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
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

    public static void Transaction_ShouldDeleteDataFromDatabase<TFileParameter>(Func<FileConnection<TFileParameter>> createFileConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
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

    public static void Transaction_ShouldUpdateDataInDatabase<TFileParameter>(Func<FileConnection<TFileParameter>> createFileConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
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

    public static void Transaction_ShouldRollbackWhenExceptionIsThrown<TFileParameter>(Func<FileConnection<TFileParameter>> createFileConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        // Arrange
        var connection = createFileConnection();
        connection.Open();

        // Start a transaction
        var transaction = connection.BeginTransaction();

        try
        {
            // Create a command to insert data into the locations table
            var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = "INSERT INTO locations (city, state) VALUES (@City, @State)";

            command.Parameters.Add(command.CreateParameter("City", "Bannu"));
            command.Parameters.Add(command.CreateParameter("State", "MA"));

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
            var adapter = connection.CreateDataAdapter("SELECT * FROM locations WHERE city = 'Bannu'");
            var dataSet = new DataSet();
            adapter.Fill(dataSet);
            Assert.Equal(0, dataSet.Tables[0].Rows.Count);
        }

        // Close the connection
        connection.Close();
    }

    public static void Transaction_MultipleInserts_GeneratingIdentity<TFileParameter>(Func<FileConnection<TFileParameter>> createFileConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        using (var connection = createFileConnection())
        {
            connection.Open();

            using (var transaction = connection.BeginTransaction())
            {
                var commandText = "INSERT INTO \"Blogs\" (\"Url\") VALUES (@p0); SELECT \"BlogId\" FROM \"Blogs\" WHERE ROW_COUNT() = 1 AND \"BlogId\"=LAST_INSERT_ID();";
                var command = connection.CreateCommand(commandText);
                command.Parameters.Add(command.CreateParameter("p0", "http://blogs.msdn.com/adonet"));
                using (var reader = command.ExecuteReader())
                {
                    // Assert
                    Assert.NotNull(reader);
                    Assert.Equal(1, reader.FieldCount);

                    //first Row
                    Assert.True(reader.Read());
                    Assert.Equal(connection.DataTypeAlwaysString ? "1" : 1m, reader["BlogId"]);

                    //There should be no second row.
                    Assert.False(reader.Read());
                }

                command.Parameters.Clear();
                command.Parameters.Add(command.CreateParameter("p0", "https://www.billboard.com/"));
                using (var reader = command.ExecuteReader())
                {
                    // Assert
                    Assert.NotNull(reader);
                    Assert.Equal(1, reader.FieldCount);

                    //first Row
                    Assert.True(reader.Read());
                    Assert.Equal(connection.DataTypeAlwaysString ? "2" : 2m, reader["BlogId"]);

                    //There should be no second row.
                    Assert.False(reader.Read());
                }

                transaction.Commit();
            }

            // Query the Blogs table to see that the rows were finally INSERT'd
            var adapter = connection.CreateDataAdapter("SELECT * FROM Blogs");
            var dataSet = new DataSet();
            adapter.Fill(dataSet);
            
            Assert.Equal(2, dataSet.Tables[0].Rows.Count);

            var firstRow = dataSet.Tables[0].Rows[0];
            Assert.Equal(connection.DataTypeAlwaysString ? "1" : 1m, firstRow["BlogId"]);
            Assert.Equal("http://blogs.msdn.com/adonet", firstRow["Url"]);

            var secondRow = dataSet.Tables[0].Rows[1];
            Assert.Equal(connection.DataTypeAlwaysString ? "2" : 2m, secondRow["BlogId"]);
            Assert.Equal("https://www.billboard.com/", secondRow["Url"]);

        }
    }
}