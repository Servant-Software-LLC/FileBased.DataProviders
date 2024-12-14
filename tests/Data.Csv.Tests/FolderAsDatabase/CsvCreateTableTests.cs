using Data.Common.Extension;
using Data.Tests.Common;
using System.Data.CsvClient;
using System.Reflection;
using Xunit;

namespace Data.Csv.Tests.FolderAsDatabase;

public class CsvCreateTableTests
{
    [Fact]
    public void CreateTable_WhenNoColumns_ShouldWork()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        CreateTableTests.CreateTable_WhenNoColumns_ShouldWork(
                       () => new CsvConnection(ConnectionStrings.Instance.FolderAsDB.Sandbox("Sandbox", sandboxId)));

    }

    [Fact]
    public void CreateTable_FollowedByInsert_ShouldWork()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        CreateTableTests.CreateTable_FollowedByInsert_ShouldWork(
                       () => new CsvConnection(ConnectionStrings.Instance.FolderAsDB.Sandbox("Sandbox", sandboxId)));

    }
}
