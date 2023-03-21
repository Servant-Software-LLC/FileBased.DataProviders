using System.Data;
using System.Data.XmlClient;
using Xunit;

namespace Data.Xml.Tests.FolderAsDatabase;

public partial class JsonDataAdapterTests
{
    [Fact]
    public void DataAdapter_ShouldFillTheDataSet()
    {
        // Arrange
        var connection = new XmlConnection(ConnectionStrings.FolderAsDBConnectionString);
        var adapter = new XmlDataAdapter("SELECT * FROM locations", connection);

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

    [Fact]
    public void Adapter_ShouldReturnData()
    {
        // Arrange
        var connection = new XmlConnection(ConnectionStrings.FolderAsDBConnectionString);
        var adapter = new XmlDataAdapter("SELECT * FROM employees", connection);

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
        Assert.Equal(56000M, dataset.Tables[0].Rows[0]["salary"]);
        Assert.IsType<decimal>(dataset.Tables[0].Rows[0]["salary"]);
        Assert.Equal(true, dataset.Tables[0].Rows[0]["married"]);
        Assert.IsType<bool>(dataset.Tables[0].Rows[0]["married"]);

        //second row
        Assert.Equal("Bob", dataset.Tables[0].Rows[1]["name"]);
        Assert.IsType<string>(dataset.Tables[0].Rows[1]["name"]);
        Assert.Equal("bob32@gmail.com", dataset.Tables[0].Rows[1]["email"]);
        Assert.IsType<string>(dataset.Tables[0].Rows[1]["email"]);
        Assert.Equal((decimal)95000, dataset.Tables[0].Rows[1]["salary"]);
        Assert.IsType<decimal>(dataset.Tables[0].Rows[1]["salary"]);
        Assert.Equal(DBNull.Value, dataset.Tables[0].Rows[1]["married"]);
        Assert.IsType<DBNull>(dataset.Tables[0].Rows[1]["married"]);

        connection.Close();
    }

    [Fact]
    public void DataAdapter_ShouldFillTheDataSet_WithFilter()
    {
        // Arrange
        var connection = new XmlConnection(ConnectionStrings.FolderAsDBConnectionString);
        var selectCommand = new XmlCommand("SELECT * FROM [locations] WHERE zip = 78132", connection);
        var dataAdapter = new XmlDataAdapter(selectCommand);
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
        Assert.Equal(78132M, row["zip"]);

        // Close the connection
        connection.Close();
    }

    [Fact]
    public void Adapter_ShouldFillDatasetWithInnerJoin()
    {
        // Arrange
        var connection = new XmlConnection(ConnectionStrings.eComDBConnectionString);
        var command = new XmlCommand("SELECT [c].[CustomerName], [o].[OrderDate], [oi].[Quantity], [p].[Name] FROM [Customers c] INNER JOIN [Orders o] ON [c].[ID] = [o].[CustomerID] INNER JOIN [OrderItems oi] ON [o].[ID] = [oi].[OrderID] INNER JOIN [Products p] ON [p].[ID] = [oi].[ProductID]", connection);
        var adapter = new XmlDataAdapter(command);
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

    [Fact]
    public void Adapter_ShouldReadDataWithSelectedColumns()
    {
        // Arrange
        var connection = new XmlConnection(ConnectionStrings.FolderAsDBConnectionString);
        var dataSet = new DataSet();

        // Act - Query two columns from the locations table
        var command = new XmlCommand("SELECT city, state FROM locations", connection);
        var adapter = new XmlDataAdapter(command);
        adapter.Fill(dataSet);

        // Assert
        var dataTable = dataSet.Tables[0];
        Assert.NotNull(dataTable);
        Assert.True(dataTable.Rows.Count > 0);
        Assert.Equal(2, dataTable.Columns.Count);

        // Act - Query two columns from the employees table
        command = new XmlCommand("SELECT name, salary FROM employees", connection);
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

    [Fact]
    public void Update_DataAdapter_Should_Update_Existing_Row()
    {
        // Arrange - create connection and commands to insert and update data
        var connection = new XmlConnection(ConnectionStrings.FolderAsDBConnectionString);
        connection.Open();

        var locationInsertCommand = new XmlCommand("INSERT INTO locations (city, state, zip) VALUES ('Boston', 'MA', '90001')", connection);
        var employeeInsertCommand = new XmlCommand("INSERT INTO employees (name, salary) VALUES ('Alice', 60)", connection);

        var locationUpdateCommand = new XmlCommand("UPDATE locations SET zip = '32655' WHERE city = 'Boston'", connection);
        var employeeUpdateCommand = new XmlCommand("UPDATE employees SET salary = 60000 WHERE name = 'Alice'", connection);

        var locationSelectCommand = new XmlCommand("SELECT city, state, zip FROM locations WHERE zip = '32655'", connection);
        var employeeSelectCommand = new XmlCommand("SELECT name, salary FROM employees WHERE name = 'Alice'", connection);

        // Act - insert a row into locations and employees tables
        locationInsertCommand.ExecuteNonQuery();
        employeeInsertCommand.ExecuteNonQuery();

        // Update the inserted row using a DataAdapter
        var adapter = new XmlDataAdapter(locationSelectCommand);
        adapter.UpdateCommand = locationUpdateCommand;
        var dataSet = new DataSet();
        adapter.Fill(dataSet);
        adapter.Update(dataSet);


        adapter = new XmlDataAdapter(employeeSelectCommand);
        adapter.UpdateCommand = employeeUpdateCommand;
        dataSet = new DataSet();
        adapter.Fill(dataSet);
        adapter.Update(dataSet);

        // Act - retrieve the updated data using a DataReader
        dataSet = new DataSet();
        adapter = new XmlDataAdapter(locationSelectCommand);
        adapter.Fill(dataSet);
        var dataTable = dataSet.Tables[0];
        Assert.Single(dataTable.Rows);
        var row = dataTable.Rows[0];
        Assert.Equal("Boston", row["city"]);
        Assert.Equal("MA", row["state"]);
        Assert.Equal(32655M, row["zip"]);

        dataSet = new DataSet();
        adapter = new XmlDataAdapter(employeeSelectCommand);
        adapter.Fill(dataSet);
        dataTable = dataSet.Tables[0];

        // Assert - check that the updated data is retrieved correctly
        Assert.Single(dataTable.Rows);
        row = dataTable.Rows[0];
        Assert.Equal("Alice", row["name"]);
        Assert.Equal(60000M, row["salary"]);

        // Close the connection
        connection.Close();
    }

    [Fact]
    public void FillSchema_ShouldReturnDataTableWithAllColumns()
    {
        // Arrange
        var dataSet = new DataSet();
        var adapter = new XmlDataAdapter();
        adapter.SelectCommand = new XmlCommand("SELECT * FROM employees", new XmlConnection(ConnectionStrings.FolderAsDBConnectionString));

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

    [Fact]
    public void FillSchema_ShouldThrowInvalidOperationException_WhenSelectCommandIsNull()
    {
        // Arrange
        var dataSet = new DataSet();
        var adapter = new XmlDataAdapter();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => adapter.FillSchema(dataSet, SchemaType.Mapped));
    }

    [Fact]
    public void FillSchema_ShouldThrowInvalidOperationException_WhenSelectCommandConnectionIsNull()
    {
        // Arrange
        var dataSet = new DataSet();
        var adapter = new XmlDataAdapter();
        adapter.SelectCommand = new XmlCommand("SELECT * FROM employees");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => adapter.FillSchema(dataSet, SchemaType.Mapped));
    }

    [Fact]
    public void FillSchema_ShouldThrowInvalidOperationException_WhenSelectCommandTextIsNullOrEmpty()
    {
        // Arrange
        var dataSet = new DataSet();
        var adapter = new XmlDataAdapter();
        adapter.SelectCommand = new XmlCommand("", new XmlConnection(ConnectionStrings.FolderAsDBConnectionString));

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => adapter.FillSchema(dataSet, SchemaType.Mapped));
    }

    [Fact]
    public void GetFillParameters_ShouldReturnCorrectParametersForQueryWithoutParameters()
    {
        // Arrange
        var connection = new XmlConnection(ConnectionStrings.eComDBConnectionString);
        var command = new XmlCommand("SELECT [Name], [Email] FROM [Customers]", connection);
        var adapter = new XmlDataAdapter(command);

        // Act
        var parameters = adapter.GetFillParameters();

        // Assert
        Assert.NotNull(parameters);
        Assert.Empty(parameters);
    }

    [Fact]
    public void GetFillParameters_ShouldReturnCorrectParametersForQueryWithParameters()
    {
        // Arrange
        var connection = new XmlConnection(ConnectionStrings.FolderAsDBConnectionString);
        var command = new XmlCommand("SELECT [Name], [Email] FROM [Employees] WHERE [married] = @married", connection);
        command.Parameters.Add(new XmlParameter("@married", true));
        var adapter = new XmlDataAdapter(command);

        // Act
        var parameters = adapter.GetFillParameters();

        // Assert
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("@married", parameters[0].ParameterName);
        Assert.Equal(true, parameters[0].Value);
    }

    [Fact]
    public void GetFillParameters_ShouldReturnEmptyParametersForNonSelectQuery()
    {
        // Arrange
        var connection = new XmlConnection(ConnectionStrings.FolderAsDBConnectionString);
        var command = new XmlCommand("INSERT INTO [Employees] ([Name], [Email]) VALUES ('Test', 'test@test.com')", connection);
        var adapter = new XmlDataAdapter(command);

        // Act
        var parameters = adapter.GetFillParameters();

        // Assert
        Assert.NotNull(parameters);
        Assert.Empty(parameters);
    }

}
