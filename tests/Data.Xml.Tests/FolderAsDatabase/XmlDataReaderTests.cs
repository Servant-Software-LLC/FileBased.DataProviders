using Data.Json.Tests.FileAsDatabase;
using System.Data.XmlClient;
using Xunit;

namespace Data.Xml.Tests.FolderAsDatabase;

/// <summary>
/// Tests that exercise the <see cref="XmlDataReader"/> class.
/// </summary>
public class XmlDataReaderTests
{
    [Fact]
    public void Reader_ShouldReadData()
    {
        DataReaderTests.Reader_ShouldReadData(() =>
        new XmlConnection(ConnectionStrings.Instance.FolderAsDB));
    }

    [Fact]
    public void Reader_ShouldReturnData()
    {
        DataReaderTests.Reader_ShouldReturnData(() =>
       new XmlConnection(ConnectionStrings.Instance.FolderAsDB));
    }

    [Fact]
    public void Reader_ShouldReturnSchemaTablesData()
    {
        DataReaderTests.Reader_ShouldReturnSchemaTablesData(() =>
            new XmlConnection(ConnectionStrings.Instance.FolderAsDB), true);
    }

    [Fact]
    public void Reader_ShouldReturnData_WithFilter()
    {
        DataReaderTests.Reader_ShouldReturnData_WithFilter(() =>
       new XmlConnection(ConnectionStrings.Instance.FolderAsDB));
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
     new XmlConnection(ConnectionStrings.Instance.FolderAsDB));
    }
}