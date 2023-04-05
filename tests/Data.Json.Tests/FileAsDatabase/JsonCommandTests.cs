using Data.Csv.Tests.FolderAsDatabase;
using System.Data.JsonClient;
using Xunit;

namespace Data.Json.Tests.FileAsDatabase;

/// <summary>
/// Tests that exercise the <see cref="JsonCommand"/> class while using the 'File as Database' approach.
/// </summary>
public class JsonCommandTests
{
    [Fact]
    public void ExecuteScalar_ShouldReturnFirstRowFirstColumn()
    {
        CommandTests.ExecuteScalar_ShouldReturnFirstRowFirstColumn(
          () => new JsonConnection(ConnectionStrings.Instance.
          FileAsDB));
    }

    [Fact]
    public void ExecuteScalar_ShouldCountRecords()
    {
        CommandTests.ExecuteScalar_ShouldCountRecords(
           () => new JsonConnection(ConnectionStrings.Instance.
           FileAsDB));
    }
}