using Data.Csv.Tests.FolderAsDatabase;
using System.Data.XmlClient;
using Xunit;

namespace Data.Xml.Tests.FolderAsDatabase;

/// <summary>
/// Tests that exercise the <see cref="XmlCommand"/> class while using the 'Folder as Database' approach.
/// </summary>
public class XmlCommandTests
{
    [Fact]
    public void ExecuteScalar_ShouldReturnFirstRowFirstColumn()
    {
        CommandTests.ExecuteScalar_ShouldReturnFirstRowFirstColumn(
           () => new XmlConnection(ConnectionStrings.Instance.
           FolderAsDBConnectionString));
    }

    [Fact]
    public void ExecuteScalar_ShouldCountRecords()
    {
        CommandTests.ExecuteScalar_ShouldCountRecords(
          () => new XmlConnection(ConnectionStrings.Instance.
          FolderAsDBConnectionString));
    }
}