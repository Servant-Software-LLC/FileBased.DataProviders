using Data.Common.Extension;
using System.Data.JsonClient;
using System.Reflection;
using Xunit;

namespace Data.Json.Tests.FileAsDatabase;

public class JsonDataReaderTests
{
    [Fact]
    public void Reader_ShouldReadData()
    {
        DataReaderTests.Reader_ShouldReadData(() =>
        new JsonConnection(ConnectionStrings.Instance.FileAsDB));
    }

    [Fact]
    public void Reader_ShouldReturnData()
    {
        DataReaderTests.Reader_ShouldReturnData(() =>
       new JsonConnection(ConnectionStrings.Instance.FileAsDB));
    }

    [Fact]
    public void Reader_Limit_ShouldReturnOnlyFirstRow()
    {
        DataReaderTests.Reader_Limit_ShouldReturnOnlyFirstRow(() =>
       new JsonConnection(ConnectionStrings.Instance.FileAsDB));
    }

    [Fact]
    public void Reader_Limit_ShouldReturnOnlySecondRow()
    {
        DataReaderTests.Reader_Limit_ShouldReturnOnlySecondRow(() =>
       new JsonConnection(ConnectionStrings.Instance.FileAsDB));
    }

    [Fact]
    public void Reader_ShouldReturnSchemaTablesData()
    {
        DataReaderTests.Reader_ShouldReturnSchemaTablesData(() =>
            new JsonConnection(ConnectionStrings.Instance.FileAsDB));
    }

    [Fact]
    public void Reader_ShouldReturnSchemaColumnsData()
    {
        DataReaderTests.Reader_ShouldReturnSchemaColumnsData(() =>
            new JsonConnection(ConnectionStrings.Instance.FileAsDB));
    }

    [Fact]
    public void Reader_ShouldReturnData_WithFilter()
    {
        DataReaderTests.Reader_ShouldReturnData_WithFilter(() =>
       new JsonConnection(ConnectionStrings.Instance.FileAsDB));
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
            new JsonConnection(ConnectionStrings.Instance.FileAsDB));
    }

    [Fact]
    public void Reader_NextResult_UpdateReturningOne()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        DataReaderTests.Reader_NextResult_UpdateReturningOne(() =>
            new JsonConnection(ConnectionStrings.Instance.FileAsDB.Sandbox("Sandbox", sandboxId)));
    }

    [Fact]
    public void Reader_NextResult_ShouldReadData()
    {
        DataReaderTests.Reader_NextResult_ShouldReadData(() =>
            new JsonConnection(ConnectionStrings.Instance.FileAsDB));
    }

    [Fact]
    public void Reader_NextResult_WithInsert()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        DataReaderTests.Reader_NextResult_WithInsert(() =>
            new JsonConnection(ConnectionStrings.Instance.FileAsDB.Sandbox("Sandbox", sandboxId)));
    }

    [Fact]
    public void Reader_NextResult_WithFunctions()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        DataReaderTests.Reader_NextResult_WithFunctions(() =>
            new JsonConnection(ConnectionStrings.Instance.FileAsDB.Sandbox("Sandbox", sandboxId)));
    }
}