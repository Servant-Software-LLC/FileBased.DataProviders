using Data.Common.DataSource;
using Data.Common.Extension;
using Data.Common.Utils.ConnectionString;
using Data.Csv.Tests.Utils;
using Data.Json.Tests.FileAsDatabase;
using Data.Tests.Common.POCOs;
using Data.Tests.Common.Utils;
using System.Data;
using System.Data.CsvClient;
using System.Reflection;
using System.Text;
using Xunit;

namespace Data.Csv.Tests.FolderAsDatabase;

/// <summary>
/// Tests that exercise the <see cref="CsvDataReader"/> class.
/// </summary>
public class CsvDataReaderTests
{
    [Fact]
    public void Reader_ShouldReadData()
    {
        DataReaderTests.Reader_ShouldReadData(() =>
            new CsvConnection(ConnectionStrings.Instance.FolderAsDB));
    }

    [Fact]
    public void Reader_ShouldReadData_StreamedDataSource()
    {
        DataReaderTests.Reader_ShouldReadData(() =>
            CustomDataSourceFactory.VirtualFolderAsDB((connectionString) => new CsvConnection(connectionString)));
    }

    [Fact]
    public async Task Reader_ShouldReadDataAsync_LargeStreamedDataSource()
    {
        await DataReaderTests.Reader_ShouldReadDataLarge(() =>
            CustomDataSourceFactory.VirtualFolderAsDB((connectionString) => new CsvConnection(connectionString), true));
    }

    [Fact]
    public void Reader_ShouldReadData_If_ColumnNames_Preceded_With_Space()
    {
        const string filePath = "Sources/sample.csv";
        const string tableName = "Table";
        DataTable dataTable = new DataTable() { TableName = tableName };

        byte[] fileBytes = File.ReadAllBytes(filePath);
        MemoryStream fileStream = new MemoryStream(fileBytes);
        var connection = new CsvConnection(FileConnectionString.CustomDataSource);
        StreamedDataSource dataSourceProvider = new(tableName, fileStream);

        connection.DataSourceProvider = dataSourceProvider;
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT * FROM {tableName}";

        var reader = command.ExecuteReader();
        dataTable.Load(reader);

        Assert.Equal(5, dataTable.Rows.Count);
    }

    [Fact]
    public void Reader_Fails_on_Id_Column()
    {
        const string filePath = "Sources/TdmTestData.csv";
        const string tableName = "Table";
        DataTable dataTable = new DataTable() { TableName = tableName };

        byte[] fileBytes = File.ReadAllBytes(filePath);
        MemoryStream fileStream = new MemoryStream(fileBytes);
        var connection = new CsvConnection(FileConnectionString.CustomDataSource);
        StreamedDataSource dataSourceProvider = new(tableName, fileStream);

        connection.DataSourceProvider = dataSourceProvider;
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT * FROM {tableName}";

        var reader = command.ExecuteReader();
        dataTable.Load(reader);

        Assert.Equal(50, dataTable.Rows.Count);
    }


    [Fact]
    public void Reader_ShouldReadData_Different_DataTypes()
    {
        const string filePath = "Sources/dataTypes.csv";
        const string tableName = "Table";
        DataTable dataTable = new DataTable() { TableName = tableName };

        byte[] fileBytes = File.ReadAllBytes(filePath);
        MemoryStream fileStream = new MemoryStream(fileBytes);
        var connection = new CsvConnection(FileConnectionString.CustomDataSource);
        StreamedDataSource dataSourceProvider = new(tableName, fileStream);

        connection.DataSourceProvider = dataSourceProvider;
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT * FROM {tableName}";

        var reader = command.ExecuteReader();
        dataTable.Load(reader);

        Assert.Equal(42, dataTable.Rows.Count);
    }

    [Fact]
    public void Reader_ShouldReadData_DataFrame_OddBehavior()
    {
        const string csvString = "Id, Name, � nAMe �\n1, Bogart, Bob";
        const string tableName = "Table";

        var connection = CsvConnectionFactory.GetCsvConnection(tableName, csvString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT * FROM {tableName}";

        var reader = command.ExecuteReader();
        DataTable dataTable = new DataTable() { TableName = tableName };
        dataTable.Load(reader);

        Assert.Equal(3, dataTable.Columns.Count);
        Assert.Equal("Id", dataTable.Columns[0].ColumnName);
        Assert.Equal("Name", dataTable.Columns[1].ColumnName);

        //NOTE: This is behavior (of appending an ordinal to the column name) of DataTable.Load that is out of our control.
        Assert.Equal("nAMe1", dataTable.Columns[2].ColumnName);

        Assert.Equal(1, dataTable.Rows.Count);
        var firstRow = dataTable.Rows[0];

        Assert.Equal(1.0, firstRow[0]);
        Assert.Equal("Bogart", firstRow[1]);
        Assert.Equal("Bob", firstRow[2]);
    }

    [Fact]
    public void Reader_ShouldReturnData()
    {
        DataReaderTests.Reader_ShouldReturnData(() =>
            new CsvConnection(ConnectionStrings.Instance.FolderAsDB));
    }

    [Fact]
    public void Reader_Limit_ShouldReturnOnlyFirstRow()
    {
        DataReaderTests.Reader_Limit_ShouldReturnOnlyFirstRow(() =>
            new CsvConnection(ConnectionStrings.Instance.FolderAsDB));
    }

    [Fact]
    public void Reader_Limit_ShouldReturnOnlySecondRow()
    {
        DataReaderTests.Reader_Limit_ShouldReturnOnlySecondRow(() =>
            new CsvConnection(ConnectionStrings.Instance.FolderAsDB));
    }

    [Fact]
    public void Reader_ShouldReturnSchemaTablesData()
    {
        DataReaderTests.Reader_ShouldReturnSchemaTablesData(() =>
            new CsvConnection(ConnectionStrings.Instance.FolderAsDB));
    }

    [Fact]
    public void Reader_ShouldReturnSchemaColumnsData()
    {
        DataReaderTests.Reader_ShouldReturnSchemaColumnsData(() =>
            new CsvConnection(ConnectionStrings.Instance.FolderAsDB));
    }

    [Fact]
    public void Reader_ShouldReturnData_WithFilter()
    {
        DataReaderTests.Reader_ShouldReturnData_WithFilter(() =>
            new CsvConnection(ConnectionStrings.Instance.FolderAsDB));
    }

    [Fact]
    public void Reader_ShouldReadDataWithInnerJoin()
    {
        DataReaderTests.Reader_ShouldReadDataWithInnerJoin(() =>
            new CsvConnection(ConnectionStrings.Instance.eComFolderDB));
    }

    [Fact]
    public void Reader_ShouldReadDataWithSelectedColumns()
    {
        DataReaderTests.Reader_ShouldReadDataWithSelectedColumns(() =>
            new CsvConnection(ConnectionStrings.Instance.FolderAsDB));
    }

    [Fact]
    public void Reader_NextResult_ShouldReadData()
    {
        DataReaderTests.Reader_NextResult_ShouldReadData(() =>
            new CsvConnection(ConnectionStrings.Instance.FolderAsDB));
    }

    [Fact]
    public void Reader_NextResult_UpdateReturningOne()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        DataReaderTests.Reader_NextResult_UpdateReturningOne(() =>
            new CsvConnection(ConnectionStrings.Instance.FolderAsDB.Sandbox("Sandbox", sandboxId)));
    }

    [Fact]
    public void Reader_NextResult_WithInsert()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        DataReaderTests.Reader_NextResult_WithInsert(() =>
            new CsvConnection(ConnectionStrings.Instance.FolderAsDB.Sandbox("Sandbox", sandboxId)));
    }

    [Fact]
    public void Reader_NextResult_WithFunctions()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        DataReaderTests.Reader_NextResult_WithFunctions(() =>
            new CsvConnection(ConnectionStrings.Instance.FolderAsDB.Sandbox("Sandbox", sandboxId)));
    }

    [Fact]
    public void Reader_TableAlias()
    {
        DataReaderTests.Reader_TableAlias(() =>
            new CsvConnection(ConnectionStrings.Instance.FolderAsDB));
    }

    [Fact]
    public void Reader_Supports_Large_Data_Files()
    {
        int counter = 0;
        const string tableName = nameof(TestRecord);

        var unendingStream = new UnendingCsvStream<TestRecord>(() =>
        {
            var record = new TestRecord
            {
                Id = counter,
                Value = $"Value {counter}"
            };
            counter++;
            return record;
        });

        var connection = new CsvConnection(FileConnectionString.CustomDataSource);
        StreamedDataSource dataSourceProvider = new(tableName, unendingStream);
        connection.DataSourceProvider = dataSourceProvider;

        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT * FROM {tableName}";
        var reader = command.ExecuteReader();

        //This should not go into an infinite loop, reading in all of the stream.
        Assert.True(reader.Read());


        Assert.Equal(2, reader.FieldCount);

        //Validate the first record
        Assert.Equal(0, reader.GetInt32(nameof(TestRecord.Id)));
        Assert.Equal("Value 0", reader.GetString(nameof(TestRecord.Value)));
    }

    [Fact]
    public void CustomGuessTypeFunction_OverridesDefaultTypeInference()
    {
        // Arrange:
        // CSV content where normally the "Value" column would be inferred as a numeric type.
        string csvContent = @"Id,Value
1,3.14
2,2.71";
        const string tableName = "Table";
        CsvConnection connection = CsvConnectionFactory.GetCsvConnection(tableName, csvContent);

        // Set the custom GuessTypeFunction so that it always returns typeof(string)
        // regardless of the sampled values.
        connection.GuessTypeFunction = (IEnumerable<string> values) => typeof(string);

        // Act:
        connection.Open();

        using var command = connection.CreateCommand();
        // The table name can be any string; our TestDataSourceProvider ignores it.
        command.CommandText = $"SELECT * FROM {tableName}";
        var reader = command.ExecuteReader();

        DataTable dt = new DataTable();
        dt.Load(reader);

        // Debug output: Uncomment to inspect column types
        // foreach (DataColumn col in dt.Columns)
        // {
        //     Console.WriteLine($"{col.ColumnName}: {col.DataType}");
        // }

        // Assert:
        // Our custom guess function should force the "Value" column to be of type string.
        Assert.True(dt.Columns.Contains("Value"));
        Assert.Equal(typeof(string), dt.Columns["Value"]!.DataType);

        // Optionally, you can also check the "Id" column.
        // Depending on your overall design, you might want both columns to be string.
        // For this test we assume the custom guess function is applied to all columns.
        Assert.True(dt.Columns.Contains("Id"));
        Assert.Equal(typeof(string), dt.Columns["Id"]!.DataType);
    }
}