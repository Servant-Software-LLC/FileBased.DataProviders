using Data.Common.Extension;
using Data.Tests.Common;
using System.Data;
using System.Data.CsvClient;
using System.Reflection;
using Xunit;

namespace Data.Csv.Tests.FolderAsDatabase
{
    public partial class CsvDataAdapterTests
    {
        [Fact]
        public void DataAdapter_ShouldFillTheDataSet()
        {
            // Arrange
            var connection = new CsvConnection(ConnectionStrings.Instance.FolderAsDBConnectionString);
            var adapter = new CsvDataAdapter("SELECT * FROM locations", connection);

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
            var connection = new CsvConnection(ConnectionStrings.Instance.FolderAsDBConnectionString);
            var adapter = new CsvDataAdapter("SELECT * FROM employees", connection);

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
            Assert.Equal(56000M, dataset.Tables[0].Rows[0]["salary"].GetValueAsType<decimal>());
            Assert.IsType<decimal>(dataset.Tables[0].Rows[0]["salary"].GetValueAsType<decimal>());
            Assert.True(dataset.Tables[0].Rows[0]["married"].GetValueAsType<bool>());
            Assert.IsType<bool>(dataset.Tables[0].Rows[0]["married"].GetValueAsType<bool>());

            //second row
            Assert.Equal("Bob", dataset.Tables[0].Rows[1]["name"]);
            Assert.IsType<string>(dataset.Tables[0].Rows[1]["name"]);
            Assert.Equal("bob32@gmail.com", dataset.Tables[0].Rows[1]["email"]);
            Assert.IsType<string>(dataset.Tables[0].Rows[1]["email"]);
            Assert.Equal((decimal)95000, dataset.Tables[0].Rows[1]["salary"].GetValueAsType<decimal>());
            Assert.IsType<decimal>(dataset.Tables[0].Rows[1]["salary"].GetValueAsType<decimal>());
            Assert.Equal(string.Empty, dataset.Tables[0].Rows[1]["married"]);

            connection.Close();
        }

        [Fact]
        public void DataAdapter_ShouldFillTheDataSet_WithFilter()
        {
            // Arrange
            var connection = new CsvConnection(ConnectionStrings.Instance.FolderAsDBConnectionString);
            var selectCommand = new CsvCommand("SELECT * FROM [locations] WHERE zip = 78132", connection);
            var dataAdapter = new CsvDataAdapter(selectCommand);
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
            Assert.Equal(78132M, row["zip"].GetValueAsType<decimal>());

            // Close the connection
            connection.Close();
        }
        
        //TODO: Once we have an eComDB then uncomment...

        //[Fact]
        //public void Adapter_ShouldFillDatasetWithInnerJoin()
        //{
        //    // Arrange
        //    var connection = new CsvConnection(ConnectionStrings.Instance.eComDBConnectionString);
        //    var command = new CsvCommand("SELECT [c].[CustomerName], [o].[OrderDate], [oi].[Quantity], [p].[Name] FROM [Customers c] INNER JOIN [Orders o] ON [c].[ID] = [o].[CustomerID] INNER JOIN [OrderItems oi] ON [o].[ID] = [oi].[OrderID] INNER JOIN [Products p] ON [p].[ID] = [oi].[ProductID]", connection);
        //    var adapter = new CsvDataAdapter(command);
        //    var dataSet = new DataSet();

        //    // Act
        //    adapter.Fill(dataSet);

        //    // Assert
        //    var table = dataSet.Tables[0];
        //    Assert.True(table.Rows.Count > 0, "No records were returned in the INNER JOINs");

        //    foreach (DataRow row in table.Rows)
        //    {
        //        Assert.NotNull(row[0]);
        //        Assert.NotNull(row[1]);
        //        Assert.NotNull(row[2]);
        //        Assert.NotNull(row[3]);
        //    }
        //}

        [Fact]
        public void Adapter_ShouldReadDataWithSelectedColumns()
        {
            // Arrange
            var connection = new CsvConnection(ConnectionStrings.Instance.FolderAsDBConnectionString);
            var dataSet = new DataSet();

            // Act - Query two columns from the locations table
            var command = new CsvCommand("SELECT city, state FROM locations", connection);
            var adapter = new CsvDataAdapter(command);
            adapter.Fill(dataSet);

            // Assert
            var dataTable = dataSet.Tables[0];
            Assert.NotNull(dataTable);
            Assert.True(dataTable.Rows.Count > 0);
            Assert.Equal(2, dataTable.Columns.Count);

            // Act - Query two columns from the employees table
            command = new CsvCommand("SELECT name, salary FROM employees", connection);
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
                () => new CsvConnection(ConnectionStrings.Instance.FolderAsDBConnectionString.Sandbox("Sandbox", sandboxId)), true);
        }

        [Fact]
        public void FillSchema_ShouldReturnDataTableWithAllColumns()
        {
            // Arrange
            var dataSet = new DataSet();
            var adapter = new CsvDataAdapter();
            adapter.SelectCommand = new CsvCommand("SELECT * FROM employees", new CsvConnection(ConnectionStrings.Instance.FolderAsDBConnectionString));

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
            var adapter = new CsvDataAdapter();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => adapter.FillSchema(dataSet, SchemaType.Mapped));
        }

        [Fact]
        public void FillSchema_ShouldThrowInvalidOperationException_WhenSelectCommandConnectionIsNull()
        {
            // Arrange
            var dataSet = new DataSet();
            var adapter = new CsvDataAdapter();
            adapter.SelectCommand = new CsvCommand("SELECT * FROM employees");

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => adapter.FillSchema(dataSet, SchemaType.Mapped));
        }

        [Fact]
        public void FillSchema_ShouldThrowInvalidOperationException_WhenSelectCommandTextIsNullOrEmpty()
        {
            // Arrange
            var dataSet = new DataSet();
            var adapter = new CsvDataAdapter();
            adapter.SelectCommand = new CsvCommand("", new CsvConnection(ConnectionStrings.Instance.FolderAsDBConnectionString));

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => adapter.FillSchema(dataSet, SchemaType.Mapped));
        }
  

        [Fact]
        public void GetFillParameters_ShouldReturnCorrectParametersForQueryWithParameters()
        {
            // Arrange
            var connection = new CsvConnection(ConnectionStrings.Instance.FolderAsDBConnectionString);
            var command = new CsvCommand("SELECT [Name], [Email] FROM [Employees] WHERE [married] = @married", connection);
            command.Parameters.Add(new CsvParameter("@married", true));
            var adapter = new CsvDataAdapter(command);

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
            var connection = new CsvConnection(ConnectionStrings.Instance.FolderAsDBConnectionString);
            var command = new CsvCommand("INSERT INTO [Employees] ([Name], [Email]) VALUES ('Test', 'test@test.com')", connection);
            var adapter = new CsvDataAdapter(command);

            // Act
            var parameters = adapter.GetFillParameters();

            // Assert
            Assert.NotNull(parameters);
            Assert.Empty(parameters);
        }




    }
}
