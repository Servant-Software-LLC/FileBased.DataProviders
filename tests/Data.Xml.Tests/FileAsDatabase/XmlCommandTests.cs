using Data.Csv.Tests.FolderAsDatabase;
using System.Data.XmlClient;
using Xunit;

namespace Data.Xml.Tests.FileAsDatabase;

/// <summary>
/// Tests that exercise the <see cref="XmlCommand"/> class while using the 'File as Database' approach.
/// </summary>
public class XmlCommandTests
{
    [Fact]
    public void ExecuteScalar_ShouldReturnFirstRowFirstColumn()
    {
        CommandTests.ExecuteScalar_ShouldReturnFirstRowFirstColumn(
           () => new XmlConnection(ConnectionStrings.Instance.
           FileAsDBConnectionString));
    }

    [Fact]
    public void ExecuteScalar_ShouldCountRecords()
    {
        CommandTests.ExecuteScalar_ShouldCountRecords(
          () => new XmlConnection(ConnectionStrings.Instance.
          FileAsDBConnectionString));
    }
}