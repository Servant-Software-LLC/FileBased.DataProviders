using Data.Csv.Tests.FolderAsDatabase;
using System.Data.JsonClient;
using Xunit;

namespace Data.Json.Tests.FolderAsDatabase;

/// <summary>
/// Tests that exercise the <see cref="JsonCommand"/> class while using the 'Folder as Database' approach.
/// </summary>
public class JsonCommandTests
{
    [Fact]
    public void ExecuteScalar_ShouldReturnFirstRowFirstColumn()
    {
        CommandTests.ExecuteScalar_ShouldReturnFirstRowFirstColumn(
           () => new JsonConnection(ConnectionStrings.Instance.
           FolderAsDB));
    }

    [Fact]
    public void ExecuteScalar_ShouldCountSchemaTableRecords()
    {
        CommandTests.ExecuteScalar_ShouldCountSchemaTableRecords(
          () => new JsonConnection(ConnectionStrings.Instance.
          FolderAsDB));
    }

}