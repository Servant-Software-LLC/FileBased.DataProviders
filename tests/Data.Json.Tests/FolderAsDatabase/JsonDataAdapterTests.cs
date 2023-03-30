using Data.Tests.Common;
using System.Data;
using System.Data.JsonClient;
using System.Reflection;
using Xunit;

namespace Data.Json.Tests.FolderAsDatabase;

public partial class JsonDataAdapterTests
{
    [Fact]
    public void DataAdapter_ShouldFillTheDataSet()
    {
        // Arrange
        var connection = new JsonConnection(ConnectionStrings.Instance.FolderAsDBConnectionString);
        var adapter = new JsonDataAdapter("SELECT * FROM locations", connection);

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
        var connection = new JsonConnection(ConnectionStrings.Instance.FolderAsDBConnectionString);
        var adapter = new JsonDataAdapter("SELECT * FROM employees", connection);

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
        var connection = new JsonConnection(ConnectionStrings.Instance.FolderAsDBConnectionString);
        var selectCommand = new JsonCommand("SELECT * FROM [locations] WHERE zip = 78132", connection);
        var dataAdapter = new JsonDataAdapter(selectCommand);
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
    public void Adapter_ShouldFillDatasetWithInnerJoinFromFolderAsDB()
    {
        DataAdapterTests.Adapter_ShouldFillDatasetWithInnerJoin(
                () => new JsonConnection(ConnectionStrings.Instance.eComFolderDBConnectionString));
    }

    [Fact]
    public void Adapter_ShouldReadDataWithSelectedColumns()
    {
        // Arrange
        var connection = new JsonConnection(ConnectionStrings.Instance.FolderAsDBConnectionString);
        var dataSet = new DataSet();

        // Act - Query two columns from the locations table
        var command = new JsonCommand("SELECT city, state FROM locations", connection);
        var adapter = new JsonDataAdapter(command);
        adapter.Fill(dataSet);

        // Assert
        var dataTable = dataSet.Tables[0];
        Assert.NotNull(dataTable);
        Assert.True(dataTable.Rows.Count > 0);
        Assert.Equal(2, dataTable.Columns.Count);

        // Act - Query two columns from the employees table
        command = new JsonCommand("SELECT name, salary FROM employees", connection);
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
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        DataAdapterTests.Update_DataAdapter_Should_Update_Existing_Row(
            () => new JsonConnection(ConnectionStrings.Instance.FolderAsDBConnectionString.Sandbox("Sandbox", sandboxId))
        );
    }

    [Fact]
    public void FillSchema_ShouldReturnDataTableWithAllColumns()
    {
        // Arrange
        var dataSet = new DataSet();
        var adapter = new JsonDataAdapter();
        adapter.SelectCommand = new JsonCommand("SELECT * FROM employees", new JsonConnection(ConnectionStrings.Instance.FolderAsDBConnectionString));

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
        var adapter = new JsonDataAdapter();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => adapter.FillSchema(dataSet, SchemaType.Mapped));
    }

    [Fact]
    public void FillSchema_ShouldThrowInvalidOperationException_WhenSelectCommandConnectionIsNull()
    {
        // Arrange
        var dataSet = new DataSet();
        var adapter = new JsonDataAdapter();
        adapter.SelectCommand = new JsonCommand("SELECT * FROM employees");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => adapter.FillSchema(dataSet, SchemaType.Mapped));
    }

    [Fact]
    public void FillSchema_ShouldThrowInvalidOperationException_WhenSelectCommandTextIsNullOrEmpty()
    {
        // Arrange
        var dataSet = new DataSet();
        var adapter = new JsonDataAdapter();
        adapter.SelectCommand = new JsonCommand("", new JsonConnection(ConnectionStrings.Instance.FolderAsDBConnectionString));

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => adapter.FillSchema(dataSet, SchemaType.Mapped));
    }

    [Fact]
    public void GetFillParameters_ShouldReturnCorrectParametersForQueryWithoutParameters()
    {
        // Arrange
        var connection = new JsonConnection(ConnectionStrings.Instance.eComFileDBConnectionString);
        var command = new JsonCommand("SELECT [Name], [Email] FROM [Customers]", connection);
        var adapter = new JsonDataAdapter(command);

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
        var connection = new JsonConnection(ConnectionStrings.Instance.FolderAsDBConnectionString);
        var command = new JsonCommand("SELECT [Name], [Email] FROM [Employees] WHERE [married] = @married", connection);
        command.Parameters.Add(new JsonParameter("@married", true));
        var adapter = new JsonDataAdapter(command);

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
        var connection = new JsonConnection(ConnectionStrings.Instance.FolderAsDBConnectionString);
        var command = new JsonCommand("INSERT INTO [Employees] ([Name], [Email]) VALUES ('Test', 'test@test.com')", connection);
        var adapter = new JsonDataAdapter(command);

        // Act
        var parameters = adapter.GetFillParameters();

        // Assert
        Assert.NotNull(parameters);
        Assert.Empty(parameters);
    }
}