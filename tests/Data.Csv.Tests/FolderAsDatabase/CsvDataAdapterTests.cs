using Data.Common.DataSource;
using Data.Common.Extension;
using Data.Common.Interfaces;
using Data.Common.Utils.ConnectionString;
using Data.Json.Tests.FileAsDatabase;
using System.Data;
using System.Data.CsvClient;
using System.Reflection;
using System.Text;
using Xunit;

namespace Data.Csv.Tests.FolderAsDatabase
{
    public partial class CsvDataAdapterTests
    {
        [Fact]
        public void Fill_ShouldPopulateTheDataSet()
        {
            DataAdapterTests.Fill_ShouldPopulateTheDataSet(
                   () => new CsvConnection(ConnectionStrings.Instance.
                   FolderAsDB));
        }

        [Fact]
        public void Adapter_ShouldReturnData()
        {
            DataAdapterTests.Adapter_ShouldReturnData(
                   () => new CsvConnection(ConnectionStrings.Instance.
                   FolderAsDB));
        }

        [Fact]
        public void Fill_ShouldPopulateTheDataSet_WithFilter()
        {
            DataAdapterTests.Fill_ShouldPopulateTheDataSet_WithFilter(
                           () => new CsvConnection(ConnectionStrings.Instance.
                           FolderAsDB));
        }

        [Fact]
        public void Adapter_ShouldFillDatasetWithInnerJoinFromFolderAsDB()
        {
            DataAdapterTests.Adapter_ShouldFillDatasetWithInnerJoin(
                    () => new CsvConnection(ConnectionStrings.Instance.eComFolderDB));
        }

        [Fact]
        public void Adapter_ShouldReadDataWithSelectedColumns()
        {
            DataAdapterTests.Adapter_ShouldReadDataWithSelectedColumns(
                         () => new CsvConnection(ConnectionStrings.Instance.
                         FolderAsDB));
        }

        [Fact]
        public void Update_DataAdapter_Should_Update_Existing_Row_LocationsTable()
        {
            var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
            DataAdapterTests.Update_DataAdapter_Should_Update_Existing_Row_LocationsTable(
                () => new CsvConnection(ConnectionStrings.Instance.FolderAsDB.Sandbox("Sandbox", sandboxId)));
        }

        [Fact]
        public void Update_DataAdapter_Should_Update_Existing_Row_EmployeesTable()
        {
            var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
            DataAdapterTests.Update_DataAdapter_Should_Update_Existing_Row_EmployeesTable(
                () => new CsvConnection(ConnectionStrings.Instance.FolderAsDB.Sandbox("Sandbox", sandboxId)));
        }

        [Fact]
        public void FillSchema_ShouldReturnDataTableWithAllColumns()
        {
            DataAdapterTests.FillSchema_ShouldReturnDataTableWithAllColumns(
                 () => new CsvConnection(ConnectionStrings.Instance.
                 FolderAsDB));
        }

        [Fact]
        public void FillSchema_ShouldThrowInvalidOperationException_WhenSelectCommandIsNull()
        {
            DataAdapterTests.FillSchema_ShouldThrowInvalidOperationException_WhenSelectCommandIsNull(
                  () => new CsvDataAdapter());
        }

        [Fact]
        public void FillSchema_ShouldThrowInvalidOperationException_WhenSelectCommandConnectionIsNull()
        {
            DataAdapterTests.FillSchema_ShouldThrowInvalidOperationException_WhenSelectCommandConnectionIsNull(
                 () => new CsvConnection(ConnectionStrings.Instance.
                 FolderAsDB));
        }

        [Fact]
        public void CreateAdapter_ShouldThrowArgumentException_WhenSelectCommandTextIsNullOrEmpty()
        {
            DataAdapterTests.CreateAdapter_ShouldThrowArgumentException_WhenSelectCommandTextIsNullOrEmpty(
                   () => new CsvConnection(ConnectionStrings.Instance.
                   FolderAsDB));
        }

        [Fact]
        public void GetFillParameters_ShouldReturnCorrectParametersForQueryWithoutParameters()
        {
            DataAdapterTests.GetFillParameters_ShouldReturnCorrectParametersForQueryWithoutParameters(
                  () => new CsvConnection(ConnectionStrings.Instance.
                  FolderAsDB));
        }

        [Fact]
        public void GetFillParameters_ShouldReturnCorrectParametersForQueryWithParameters()
        {
            DataAdapterTests.GetFillParameters_ShouldReturnCorrectParametersForQueryWithParameters(
                   () => new CsvConnection(ConnectionStrings.Instance.
                   FolderAsDB));
        }

        [Fact]
        public void GetFillParameters_ShouldReturnEmptyParametersForNonSelectQuery()
        {
            DataAdapterTests.GetFillParameters_ShouldReturnEmptyParametersForNonSelectQuery(
                   () => new CsvConnection(ConnectionStrings.Instance.
                   FolderAsDB));
        }

        [Fact]
        public void Fill_DataSet_DuplicateColumnNamesOfDifferingCase_AreRespected()
        {
            const string csvString = "Id, Name,   nAMe  \n1, Bogart, Bob";
            const string tableName = "Table";

            byte[] fileBytes = Encoding.UTF8.GetBytes(csvString);
            MemoryStream fileStream = new MemoryStream(fileBytes);
            var connection = new CsvConnection(FileConnectionString.CustomDataSource);
            StreamedDataSource dataSourceProvider = new(tableName, fileStream);

            connection.DataSourceProvider = dataSourceProvider;
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = $"SELECT * FROM {tableName}";

            var adapter = command.CreateAdapter();

            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);

            var dataTable = dataSet.Tables[0];
            Assert.Equal(3, dataTable.Columns.Count);
            Assert.Equal("Id", dataTable.Columns[0].ColumnName);
            Assert.Equal("Name", dataTable.Columns[1].ColumnName);
            Assert.True(
                "nAMe" == dataTable.Columns[2].ColumnName,
                $"Expected: nAMe\nActual: {dataTable.Columns[2].ColumnName}\nCharacter details: {GetCharacterDetails(dataTable.Columns[2].ColumnName)}"
            );

            Assert.Equal(1, dataTable.Rows.Count);
            var firstRow = dataTable.Rows[0];

            Assert.Equal(1.0, firstRow[0]);
            Assert.Equal("Bogart", firstRow[1]);
            Assert.Equal("Bob", firstRow[2]);

        }

        [Fact]
        public void Fill_Table_DuplicateColumnNamesOfDifferingCase_AreRespected()
        {
            const string csvString = "Id, Name,   nAMe  \n1, Bogart, Bob";
            const string tableName = "Table";

            byte[] fileBytes = Encoding.UTF8.GetBytes(csvString);
            MemoryStream fileStream = new MemoryStream(fileBytes);
            IFileConnection connection = new CsvConnection(FileConnectionString.CustomDataSource);
            StreamedDataSource dataSourceProvider = new(tableName, fileStream);

            connection.DataSourceProvider = dataSourceProvider;
            connection.Open();

            using var adapter = connection.CreateDataAdapter($"SELECT * FROM {tableName}");

            DataTable dataTable = new DataTable(tableName);
            adapter.Fill(dataTable);

            Assert.Equal(tableName, dataTable.TableName);

            Assert.Equal(3, dataTable.Columns.Count);
            Assert.Equal("Id", dataTable.Columns[0].ColumnName);
            Assert.Equal("Name", dataTable.Columns[1].ColumnName);
            Assert.Equal("nAMe", dataTable.Columns[2].ColumnName);

            Assert.Equal(1, dataTable.Rows.Count);
            var firstRow = dataTable.Rows[0];

            Assert.Equal(1.0, firstRow[0]);
            Assert.Equal("Bogart", firstRow[1]);
            Assert.Equal("Bob", firstRow[2]);

        }

        [Fact]
        public void Fill_Table_DoubleValues_SameAsData()
        {
            const string csvString = "Id,ProductName,Category,Amount,IsActive,Point,Initials,Today,NullFiled\n0.6864690584930528,Handmade Plastic Car,rich,67,True,0.16745076678158033,C,06/30/1955 00:00:00,";
            const string tableName = "Table";

            byte[] fileBytes = Encoding.UTF8.GetBytes(csvString);
            MemoryStream fileStream = new MemoryStream(fileBytes);
            IFileConnection connection = new CsvConnection(FileConnectionString.CustomDataSource);
            StreamedDataSource dataSourceProvider = new(tableName, fileStream);

            connection.DataSourceProvider = dataSourceProvider;
            connection.Open();

            using var adapter = connection.CreateDataAdapter($"SELECT * FROM {tableName}");

            DataTable dataTable = new DataTable(tableName);
            adapter.Fill(dataTable);

            Assert.Equal(tableName, dataTable.TableName);

            Assert.Equal(9, dataTable.Columns.Count);
            Assert.Equal(typeof(double), dataTable.Columns[0].DataType);
            Assert.Equal(typeof(double), dataTable.Columns[5].DataType);

            Assert.Equal(1, dataTable.Rows.Count);
            var firstRow = dataTable.Rows[0];

            Assert.Equal(0.6864690584930528, firstRow[0]);
            Assert.Equal(0.16745076678158033, firstRow[5]);
        }

        string GetCharacterDetails(string input)
        {
            return string.Join(", ", input.Select(c => $"'{c}' (U+{(int)c:X4})"));
        }
    }

}