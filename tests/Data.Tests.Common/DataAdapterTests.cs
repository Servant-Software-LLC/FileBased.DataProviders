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
}
