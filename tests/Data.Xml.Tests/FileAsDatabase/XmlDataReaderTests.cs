using Data.Common.Extension;
using Data.Tests.Common.FileAsDatabase;
using System.Data.XmlClient;
using System.Reflection;
using Xunit;

namespace Data.Xml.Tests.FileAsDatabase;

public class XmlDataReaderTests
{
    [Fact]
    public void Reader_ShouldReadData()
    {
        DataReaderTests.Reader_ShouldReadData(() =>
            new XmlConnection(ConnectionStrings.Instance.FileAsDB));
    }

    [Fact]
    public void Reader_ShouldReturnData()
    {
        DataReaderTests.Reader_ShouldReturnData(() =>
            new XmlConnection(ConnectionStrings.Instance.FileAsDB));
    }

    [Fact]
    public void Reader_Limit_ShouldReturnOnlyFirstRow()
    {
        DataReaderTests.Reader_Limit_ShouldReturnOnlyFirstRow(() =>
            new XmlConnection(ConnectionStrings.Instance.FileAsDB));
    }

    [Fact]
    public void Reader_Limit_ShouldReturnOnlySecondRow()
    {
        DataReaderTests.Reader_Limit_ShouldReturnOnlySecondRow(() =>
            new XmlConnection(ConnectionStrings.Instance.FileAsDB));
    }

    [Fact]
    public void Reader_ShouldReturnSchemaTablesData()
    {
        DataReaderTests.Reader_ShouldReturnSchemaTablesData(() =>
            new XmlConnection(ConnectionStrings.Instance.FileAsDB));
    }

    [Fact]
    public void Reader_ShouldReturnSchemaColumnsData()
    {
        DataReaderTests.Reader_ShouldReturnSchemaColumnsData(() =>
            new XmlConnection(ConnectionStrings.Instance.FileAsDB));
    }

    [Fact]
    public void Reader_ShouldReturnData_WithFilter()
    {
        DataReaderTests.Reader_ShouldReturnData_WithFilter(() =>
            new XmlConnection(ConnectionStrings.Instance.FileAsDB));
    }

    [Fact]
    public void Reader_ShouldReadDataWithInnerJoin()
    {
        DataReaderTests.Reader_ShouldReadDataWithInnerJoin(() =>
            new XmlConnection(ConnectionStrings.Instance.eComFileDB));
    }

    [Fact]
    public void Reader_ShouldReadDataWithSelectedColumns()
    {
        DataReaderTests.Reader_ShouldReadDataWithSelectedColumns(() =>
            new XmlConnection(ConnectionStrings.Instance.FileAsDB));
    }

    [Fact]
    public void Reader_NextResult_ShouldReadData()
    {
        DataReaderTests.Reader_NextResult_ShouldReadData(() =>
            new XmlConnection(ConnectionStrings.Instance.FileAsDB));
    }

    [Fact]
    public void Reader_NextResult_UpdateReturningOne()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        DataReaderTests.Reader_NextResult_UpdateReturningOne(() =>
            new XmlConnection(ConnectionStrings.Instance.FileAsDB.Sandbox("Sandbox", sandboxId)));
    }

    [Fact]
    public void Reader_NextResult_WithInsert()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        DataReaderTests.Reader_NextResult_WithInsert(() =>
        new XmlConnection(ConnectionStrings.Instance.FileAsDB.Sandbox("Sandbox", sandboxId)));
    }

    [Fact]
    public void Reader_NextResult_WithFunctions()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        DataReaderTests.Reader_NextResult_WithFunctions(() =>
        new XmlConnection(ConnectionStrings.Instance.FileAsDB.Sandbox("Sandbox", sandboxId)));
    }
}