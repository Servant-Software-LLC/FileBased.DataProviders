using System.Data.JsonClient;
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
    public void Reader_ShouldReturnSchemaTablesData()
    {
        DataReaderTests.Reader_ShouldReturnSchemaTablesData(() =>
            new JsonConnection(ConnectionStrings.Instance.FileAsDB));
    }

    [Fact]
    public void Reader_ShouldReturnSchemaColumnsData()
    {
        DataReaderTests.Reader_ShouldReturnSchemaColumnsData(() =>
            new JsonConnection(ConnectionStrings.Instance.FileAsDB), false);
    }

    [Fact]
    public void Reader_ShouldReturnData_WithFilter()
    {
        DataReaderTests.Reader_ShouldReturnData_WithFilter(() =>
       new JsonConnection(ConnectionStrings.Instance.FileAsDB));
    }

    [Fact]
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
    public void Reader_NextResult_ShouldReadData()
    {
        DataReaderTests.Reader_NextResult_ShouldReadData(() =>
        new JsonConnection(ConnectionStrings.Instance.FileAsDB));
    }
}