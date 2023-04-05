using System.Data.CsvClient;
using Xunit;

namespace Data.Csv.Tests.FolderAsDatabase;

/// <summary>
/// Tests that exercise the <see cref="CsvCommand"/> class while using the 'Folder as Database' approach.
/// </summary>
public class CsvCommandTests
{
    [Fact]
    public void ExecuteScalar_ShouldReturnFirstRowFirstColumn()
    {
        CommandTests.ExecuteScalar_ShouldReturnFirstRowFirstColumn(
            () => new CsvConnection(ConnectionStrings.Instance.
            FolderAsDBConnectionString));
    }

    [Fact]
    public void ExecuteScalar_ShouldCountRecords()
    {
        CommandTests.ExecuteScalar_ShouldCountRecords(
           () => new CsvConnection(ConnectionStrings.Instance.
           FolderAsDBConnectionString));
    }
}