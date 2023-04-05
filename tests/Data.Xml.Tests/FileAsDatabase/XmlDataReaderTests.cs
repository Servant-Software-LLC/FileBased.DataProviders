using Data.Json.Tests.FileAsDatabase;
using System.Data.XmlClient;
using Xunit;

namespace Data.Xml.Tests.FileAsDatabase;

public class XmlDataReaderTests
{
    [Fact]
    public void Reader_ShouldReadData()
    {
        DataReaderTests.Reader_ShouldReadData(() =>
        new XmlConnection(ConnectionStrings.Instance
        .FileAsDBConnectionString));
    }

    [Fact]
    public void Reader_ShouldReturnData()
    {
        DataReaderTests.Reader_ShouldReturnData(() =>
       new XmlConnection(ConnectionStrings.Instance
       .FileAsDBConnectionString));
    }

    [Fact]
    public void Reader_ShouldReturnData_WithFilter()
    {
        DataReaderTests.Reader_ShouldReturnData_WithFilter(() =>
       new XmlConnection(ConnectionStrings.Instance
       .FileAsDBConnectionString));
    }

    [Fact]
    public void Reader_ShouldReadDataWithInnerJoin()
    {
        DataReaderTests.Reader_ShouldReadDataWithInnerJoin(() =>
       new XmlConnection(ConnectionStrings.Instance
       .eComFileDBConnectionString));
    }

    [Fact]
    public void Reader_ShouldReadDataWithSelectedColumns()
    {
        DataReaderTests.Reader_ShouldReadDataWithSelectedColumns(() =>
     new XmlConnection(ConnectionStrings.Instance
     .FileAsDBConnectionString));
    }
}