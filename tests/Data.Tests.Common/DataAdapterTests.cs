using System.Data;
using System.Data.FileClient;
using Xunit;

namespace Data.Json.Tests.FileAsDatabase;

public static class DataAdapterTests
{
    public static void Update_DataAdapter_Should_Update_Existing_Row<TFileParameter>(Func<FileConnection<TFileParameter>> createFileConnection, bool dataTypesAreAlwaysString = false)
        where TFileParameter : FileParameter<TFileParameter>, new()
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

    public static void Adapter_ShouldFillDatasetWithInnerJoinFileAsDB<TFileParameter>(Func<FileConnection<TFileParameter>> createFileConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
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

    public static void Adapter_ShouldFillDatasetWithInnerJoin<TFileParameter>(Func<FileConnection<TFileParameter>> createFileConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        // Arrange
        string query = "SELECT [c].[CustomerName], [o].[OrderDate], [oi].[Quantity], [p].[Name] " +
            "FROM [Customers c] " +
            "INNER JOIN [Orders o] ON [c].[ID] = [o].[CustomerID] " +
            "INNER JOIN [OrderItems oi] ON [o].[ID] = [oi].[OrderID] " +
            "INNER JOIN [Products p] ON [p].[ID] = [oi].[ProductID]";

        // Act
        using (var connection = createFileConnection())
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = query;
                using (var adapter = command.CreateAdapter())
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

                    foreach (DataRow row in table.Rows)
                    {
                        Assert.NotNull(row[0]);
                        Assert.NotNull(row[1]);
                        Assert.NotNull(row[2]);
                        Assert.NotNull(row[3]);
                    }

                }
            }
        }
    }

    public static void DataAdapter_ShouldFillTheDataSet<TFileParameter>(Func<FileConnection<TFileParameter>> createFileConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        // Arrange
        var connection = createFileConnection();
        var adapter = connection.CreateDataAdapter("SELECT * FROM locations");

        // Act
        var dataset = new DataSet();
        adapter.Fill(dataset);

        // Assert
        Assert.True(dataset.Tables[0].Rows.Count > 0);
        Assert.Equal(4, dataset.Tables[0].Columns.Count);

        // Fill the employees table
        adapter.SelectCommand!.CommandText = "SELECT * FROM employees";
        adapter.Fill(dataset);

        // Assert
        Assert.True(dataset.Tables[0].Rows.Count > 0);
        Assert.Equal(4, dataset.Tables[0].Columns.Count);

        // Close the connection
        connection.Close();
    }

    public static void Adapter_ShouldReturnData<TFileParameter>(Func<FileConnection<TFileParameter>> createFileConnection, bool dataTypesAreAlwaysString = false)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        // Arrange
        var connection = createFileConnection();
        var adapter = connection.CreateDataAdapter("SELECT * FROM employees");

        // Act
        var dataset = new DataSet();
        adapter.Fill(dataset);

        // Assert
        Assert.NotNull(dataset);
        Assert.Equal(4, dataset.Tables[0].Columns.Count);

        //first Row
        Assert.Equal("Joe", dataset.Tables[0].Rows[0]["name"]);
        Assert.IsType<string>(dataset.Tables![0].Rows[0]["name"]);
        Assert.Equal("Joe@gmail.com", dataset.Tables![0].Rows[0]["email"]);
        Assert.IsType<string>(dataset.Tables[0].Rows[0]["email"]);
        Assert.Equal(dataTypesAreAlwaysString ? "56000" : 56000M, dataset.Tables[0].Rows[0]["salary"]);
        Assert.Equal(dataTypesAreAlwaysString ? "True" : true, dataset.Tables[0].Rows[0]["married"]);

        //second row
        Assert.Equal("Bob", dataset.Tables[0].Rows[1]["name"]);
        Assert.IsType<string>(dataset.Tables[0].Rows[1]["name"]);
        Assert.Equal("bob32@gmail.com", dataset.Tables[0].Rows[1]["email"]);
        Assert.IsType<string>(dataset.Tables[0].Rows[1]["email"]);
        Assert.Equal(dataTypesAreAlwaysString ? "95000" : 95000M, dataset.Tables[0].Rows[1]["salary"]);
        Assert.Equal(dataTypesAreAlwaysString ? string.Empty : DBNull.Value, dataset.Tables[0].Rows[1]["married"]);
        connection.Close();
    }

    public static void DataAdapter_ShouldFillTheDataSet_WithFilter<TFileParameter>(Func<FileConnection<TFileParameter>> createFileConnection, bool dataTypesAreAlwaysString = false)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        // Arrange
        var connection = createFileConnection();
        var selectCommand = connection.CreateCommand("SELECT * FROM [locations] WHERE zip = 78132");
        var dataAdapter = selectCommand.CreateAdapter();
        dataAdapter.SelectCommand = selectCommand;
        var dataSet = new DataSet();

        // Act
        connection.Open();
        dataAdapter.Fill(dataSet);

        // Assert
        Assert.NotEmpty(dataSet.Tables);
        var locationsTable = dataSet.Tables[0];
        Assert.Equal(4, locationsTable.Columns.Count);
        Assert.Equal(1, locationsTable.Rows.Count);

        var row = locationsTable.Rows[0];
        Assert.Equal("New Braunfels", row["city"]);
        Assert.Equal(dataTypesAreAlwaysString ? "78132" : 78132M, row["zip"]);

        // Close the connection
        connection.Close();
    }

    public static void Adapter_ShouldReadDataWithSelectedColumns<TFileParameter>(Func<FileConnection<TFileParameter>> createFileConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        // Arrange
        var connection = createFileConnection();
        var dataSet = new DataSet();

        // Act - Query two columns from the locations table
        var command = connection.CreateCommand("SELECT city, state FROM locations");
        var adapter = command.CreateAdapter();
        adapter.Fill(dataSet);

        // Assert
        var dataTable = dataSet.Tables[0];
        Assert.NotNull(dataTable);
        Assert.True(dataTable.Rows.Count > 0);
        Assert.Equal(2, dataTable.Columns.Count);

        // Act - Query two columns from the employees table
        command = connection.CreateCommand("SELECT name, salary FROM employees");
        adapter.SelectCommand = command;
        adapter.Fill(dataSet);

        // Assert
        dataTable = dataSet.Tables[0];
        Assert.NotNull(dataTable);
        Assert.True(dataTable.Rows.Count > 0);
        Assert.Equal(2, dataTable.Columns.Count);

        // Close the connection
        connection.Close();
    }

    public static void FillSchema_ShouldReturnDataTableWithAllColumns<TFileParameter>(Func<FileConnection<TFileParameter>> createFileConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        // Arrange
        var dataSet = new DataSet();
        var connection = createFileConnection();
        var adapter = connection.CreateDataAdapter("SELECT * FROM employees");
        // Act
        var tables = adapter.FillSchema(dataSet, SchemaType.Source);

        // Assert
        Assert.Single(tables);
        Assert.Equal(4, tables[0].Columns.Count);
        Assert.Equal("name", tables[0].Columns[0].ColumnName);
        Assert.Equal("email", tables[0].Columns[1].ColumnName);
        Assert.Equal("salary", tables[0].Columns[2].ColumnName);
        Assert.Equal("married", tables[0].Columns[3].ColumnName);
    }

    public static void FillSchema_ShouldThrowInvalidOperationException_WhenSelectCommandIsNull<TFileParameter>(Func<FileDataAdapter<TFileParameter>> createFileDataAdapter)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        // Arrange
        var dataSet = new DataSet();
        var adapter = createFileDataAdapter();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => adapter.FillSchema(dataSet, SchemaType.Mapped));
    }

    public static void FillSchema_ShouldThrowInvalidOperationException_WhenSelectCommandConnectionIsNull<TFileParameter>(Func<FileConnection<TFileParameter>> createFileConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        // Arrange
        var dataSet = new DataSet();
        var connection = createFileConnection();
        var selectCommand = connection.CreateCommand("SELECT * FROM employees");
        var adapter = selectCommand.CreateAdapter();

        adapter.SelectCommand.Connection = null;

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => adapter.FillSchema(dataSet, SchemaType.Mapped));
    }

    public static void CreateAdapter_ShouldThrowArgumentException_WhenSelectCommandTextIsNullOrEmpty<TFileParameter>(Func<FileConnection<TFileParameter>> createFileConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        // Arrange
        var dataSet = new DataSet();
        var connection = createFileConnection();
        var selectCommand = connection.CreateCommand("");
        // Act & Assert
        Assert.Throws<ArgumentException>(() => selectCommand.CreateAdapter());
    }

    public static void GetFillParameters_ShouldReturnCorrectParametersForQueryWithoutParameters<TFileParameter>(Func<FileConnection<TFileParameter>> createFileConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        // Arrange
        var connection = createFileConnection();
        var command = connection.CreateCommand("SELECT [Name], [Email] FROM [Customers]");
        var adapter = command.CreateAdapter();

        // Act
        var parameters = adapter.GetFillParameters();

        // Assert
        Assert.NotNull(parameters);
        Assert.Empty(parameters);
    }

    public static void GetFillParameters_ShouldReturnCorrectParametersForQueryWithParameters<TFileParameter>(Func<FileConnection<TFileParameter>> createFileConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        // Arrange
        var connection = createFileConnection();
        var command = connection.CreateCommand("SELECT [Name], [Email] FROM [Customers] WHERE [ID] = @ID");
        command.Parameters.Add(command.CreateParameter("@ID", 1));
        var adapter = command.CreateAdapter();

        // Act
        var parameters = adapter.GetFillParameters();

        // Assert
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("@ID", parameters[0].ParameterName);
        Assert.Equal(1, parameters[0].Value);
    }

    public static void GetFillParameters_ShouldReturnEmptyParametersForNonSelectQuery<TFileParameter>(Func<FileConnection<TFileParameter>> createFileConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        // Arrange
        var connection = createFileConnection();
        var command = connection.CreateCommand("INSERT INTO [Customers] ([Name], [Email]) VALUES ('Test', 'test@test.com')");
        var adapter = command.CreateAdapter();

        // Act
        var parameters = adapter.GetFillParameters();

        // Assert
        Assert.NotNull(parameters);
        Assert.Empty(parameters);
    }
}