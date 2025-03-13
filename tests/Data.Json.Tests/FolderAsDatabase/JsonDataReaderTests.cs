using Data.Common.Extension;
using Data.Json.Tests.FileAsDatabase;
using Data.Json.Tests.Utils;
using Data.Tests.Common.POCOs;
using Data.Tests.Common.Utils;
using System.Data.JsonClient;
using System.Reflection;
using Xunit;

namespace Data.Json.Tests.FolderAsDatabase;

/// <summary>
/// Tests that exercise the <see cref="JsonDataReader"/> class.
/// </summary>
public class JsonDataReaderTests
{
    [Fact]
    public void Reader_ShouldReadData()
    {
        DataReaderTests.Reader_ShouldReadData(() =>
        new JsonConnection(ConnectionStrings.Instance.FolderAsDB));
    }

    [Fact]
    public void Reader_ShouldReadData_StreamedDataSource()
    {
        DataReaderTests.Reader_ShouldReadData(() =>
            CustomDataSourceFactory.VirtualFolderAsDB((connectionString) => new JsonConnection(connectionString)));
    }

    [Fact]
    public void Reader_ShouldReturnData()
    {
        DataReaderTests.Reader_ShouldReturnData(() =>
       new JsonConnection(ConnectionStrings.Instance.FolderAsDB));
    }

    [Fact]
    public void Reader_Limit_ShouldReturnOnlyFirstRow()
    {
        DataReaderTests.Reader_Limit_ShouldReturnOnlyFirstRow(() =>
       new JsonConnection(ConnectionStrings.Instance.FolderAsDB));
    }

    [Fact]
    public void Reader_Limit_ShouldReturnOnlySecondRow()
    {
        DataReaderTests.Reader_Limit_ShouldReturnOnlySecondRow(() =>
       new JsonConnection(ConnectionStrings.Instance.FolderAsDB));
    }

    [Fact]
    public void Reader_ShouldReturnSchemaTablesData()
    {
        DataReaderTests.Reader_ShouldReturnSchemaTablesData(() =>
            new JsonConnection(ConnectionStrings.Instance.FolderAsDB));
    }

    [Fact]
    public void Reader_ShouldReturnSchemaColumnsData()
    {
        DataReaderTests.Reader_ShouldReturnSchemaColumnsData(() =>
            new JsonConnection(ConnectionStrings.Instance.FolderAsDB));
    }

    [Fact]
    public void Reader_ShouldReturnData_WithFilter()
    {
        DataReaderTests.Reader_ShouldReturnData_WithFilter(() =>
       new JsonConnection(ConnectionStrings.Instance.FolderAsDB));
    }

    [Fact(Skip = "Temp: Not needed for current goal")]
    public void Reader_ShouldReadDataWithInnerJoin()
    {
        DataReaderTests.Reader_ShouldReadDataWithInnerJoin(() =>
       new JsonConnection(ConnectionStrings.Instance.eComFileDB));
    }

    [Fact]
    public void Reader_ShouldReadDataWithSelectedColumns()
    {
        DataReaderTests.Reader_ShouldReadDataWithSelectedColumns(() =>
            new JsonConnection(ConnectionStrings.Instance.FolderAsDB));
    }

    [Fact]
    public void Reader_NextResult_ShouldReadData()
    {
        DataReaderTests.Reader_NextResult_ShouldReadData(() =>
            new JsonConnection(ConnectionStrings.Instance.FolderAsDB));
    }

    [Fact]
    public void Reader_NextResult_UpdateReturningOne()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        DataReaderTests.Reader_NextResult_UpdateReturningOne(() =>
            new JsonConnection(ConnectionStrings.Instance.FolderAsDB.Sandbox("Sandbox", sandboxId)));
    }

    [Fact]
    public void Reader_NextResult_WithInsert()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        DataReaderTests.Reader_NextResult_WithInsert(() =>
        new JsonConnection(ConnectionStrings.Instance.FolderAsDB.Sandbox("Sandbox", sandboxId)));
    }

    [Fact]
    public void Reader_NextResult_WithFunctions()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        DataReaderTests.Reader_NextResult_WithFunctions(() =>
        new JsonConnection(ConnectionStrings.Instance.FolderAsDB.Sandbox("Sandbox", sandboxId)));
    }

    [Fact]
    public void Reader_Supports_Large_Data_Files()
    {
        int counter = 0;

        var unendingStream = new UnendingJsonStream<TestRecord>(() =>
        {
            var record = new TestRecord
            {
                Id = counter,
                Value = $"Value {counter}"
            };
            counter++;
            return record;
        });

        DataReaderTests.Reader_Supports_Large_Data_Files(() =>
            new JsonConnection(ConnectionStrings.Instance.FolderAsDB), unendingStream);
    }

    //
    //Json specific tests
    //

    [Fact]
    public void Reader_JsonFileContainsTrailingComma()
    {
        var connectionString = ConnectionStrings.Instance.withTrailingCommaDB;
        using (var connection = new JsonConnection(connectionString))
        {
            //Act
            connection.Open();

            var command = connection.CreateCommand("SELECT * FROM GeneralSettings");
            var reader = command.ExecuteReader();
        }

    }

}