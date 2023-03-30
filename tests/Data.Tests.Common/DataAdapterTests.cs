using System.Data;
using System.Data.FileClient;
using Xunit;

namespace Data.Tests.Common;

public static class DataAdapterTests
{
    public static void Update_DataAdapter_Should_Update_Existing_Row(Func<FileConnection> createFileConnection, bool dataTypesAreAlwaysString = false)
    {
        // Arrange - create connection and commands to insert and update data
        var connection = createFileConnection();
        connection.Open();

        var locationInsertCommand = connection.CreateCommand("INSERT INTO locations (city, state, zip) VALUES ('Boston', 'MA', '90001')");
        var employeeInsertCommand = connection.CreateCommand("INSERT INTO employees (name, salary) VALUES ('Alice', 60)");

        var locationUpdateCommand = connection.CreateCommand("UPDATE locations SET zip = '32655' WHERE city = 'Boston'");
        var employeeUpdateCommand = connection.CreateCommand("UPDATE employees SET salary = 60000 WHERE name = 'Alice'");

        var locationSelectCommand = connection.CreateCommand("SELECT city, state, zip FROM locations WHERE zip = '32655'");
        var employeeSelectCommand = connection.CreateCommand("SELECT name, salary FROM employees WHERE name = 'Alice'");

        // Act - insert a row into locations and employees tables
        locationInsertCommand.ExecuteNonQuery();
        employeeInsertCommand.ExecuteNonQuery();

        // Update the inserted row using a DataAdapter
        var adapter = locationSelectCommand.CreateAdapter();
        adapter.UpdateCommand = locationUpdateCommand;
        var dataSet = new DataSet();
        adapter.Fill(dataSet);
        adapter.Update(dataSet);

        adapter = employeeSelectCommand.CreateAdapter();
        adapter.UpdateCommand = employeeUpdateCommand;
        dataSet = new DataSet();
        adapter.Fill(dataSet);
        adapter.Update(dataSet);

        // Act - retrieve the updated data using a DataReader
        dataSet = new DataSet();
        adapter = locationSelectCommand.CreateAdapter();
        adapter.Fill(dataSet);
        var dataTable = dataSet.Tables[0];
        Assert.Single(dataTable.Rows);
        var row = dataTable.Rows[0];
        Assert.Equal("Boston", row["city"]);
        Assert.Equal("MA", row["state"]);
        Assert.Equal(dataTypesAreAlwaysString ? "32655" : 32655M, row["zip"]);

        dataSet = new DataSet();
        adapter = employeeSelectCommand.CreateAdapter();
        adapter.Fill(dataSet);
        dataTable = dataSet.Tables[0];

        // Assert - check that the updated data is retrieved correctly
        Assert.Single(dataTable.Rows);
        row = dataTable.Rows[0];
        Assert.Equal("Alice", row["name"]);
        Assert.Equal(dataTypesAreAlwaysString ? "60000" : 60000M, row["salary"]);

        // Close the connection
        connection.Close();
    }

    public static void Adapter_ShouldFillDatasetWithInnerJoinFileAsDB(Func<FileConnection> createFileConnection)
    {
        // Arrange
        var connection = createFileConnection(); ;
        var command = connection.CreateCommand();
        command.CommandText = "SELECT [c].[CustomerName], [o].[OrderDate], [oi].[Quantity], [p].[Name] FROM [Customers c] INNER JOIN [Orders o] ON [c].[ID] = [o].[CustomerID] INNER JOIN [OrderItems oi] ON [o].[ID] = [oi].[OrderID] INNER JOIN [Products p] ON [p].[ID] = [oi].[ProductID]";
        var adapter = command.CreateAdapter();
        var dataSet = new DataSet();
        // Act
        adapter.Fill(dataSet);
        // Assert
        var table = dataSet.Tables[0];
        Assert.True(table.Rows.Count > 0, "No records were returned in the INNER JOINs");

        foreach (DataRow row in table.Rows)
        {
            Assert.NotNull(row[0]);
            Assert.NotNull(row[1]);
            Assert.NotNull(row[2]);
            Assert.NotNull(row[3]);
        }
    }

    public static void Adapter_ShouldFillDatasetWithInnerJoin(Func<FileConnection> createFileConnection)
    {
        // Arrange
        string query = "SELECT [c].[CustomerName], [o].[OrderDate], [oi].[Quantity], [p].[Name] " +
            "FROM [Customers c] " +
            "INNER JOIN [Orders o] ON [c].[ID] = [o].[CustomerID] " +
            "INNER JOIN [OrderItems oi] ON [o].[ID] = [oi].[OrderID] " +
            "INNER JOIN [Products p] ON [p].[ID] = [oi].[ProductID]";

        // Act
        using (FileConnection connection = createFileConnection())
        {
            connection.Open();
            using (FileCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                using (FileDataAdapter adapter = command.CreateAdapter())
                {
                    DataSet database = new DataSet();
                    adapter.Fill(database);
                    DataTable table = database.Tables[0];

                    // Assert
                    Assert.NotNull(table);
                    Assert.Equal(40, table.Rows.Count);
                    Assert.Equal(4, table.Columns.Count);
                    Assert.Equal("John Doe", table.Rows[0]["CustomerName"].ToString());
                    Assert.Equal(new DateTime(2022, 3, 20), DateTime.Parse(table.Rows[0]!["OrderDate"].ToString()));
                    Assert.Equal(2, Convert.ToInt32(table.Rows[0]["Quantity"]));
                    Assert.Equal("Macbook Pro 13", table.Rows[0]["Name"].ToString());
                }
            }
        }
    }
}