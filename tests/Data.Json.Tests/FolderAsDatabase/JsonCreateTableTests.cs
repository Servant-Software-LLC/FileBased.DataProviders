using Data.Common.Extension;
using Data.Tests.Common;
using System.Data.JsonClient;
using System.Reflection;
using Xunit;

namespace Data.Json.Tests.FolderAsDatabase;

public class JsonCreateTableTests
{
    [Fact]
    public void CreateTable_WhenNoColumns_ShouldWork()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        CreateTableTests.CreateTable_WhenNoColumns_ShouldWork(
                       () => new JsonConnection(ConnectionStrings.Instance.FolderAsDB.Sandbox("Sandbox", sandboxId)));

    }

    [Fact]
    public void CreateTable_FollowedByInsert_ShouldWork()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        CreateTableTests.CreateTable_FollowedByInsert_ShouldWork(
                       () => new JsonConnection(ConnectionStrings.Instance.FolderAsDB.Sandbox("Sandbox", sandboxId)));

    }
}
