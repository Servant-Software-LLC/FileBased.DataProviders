using Data.Tests.Common;
using Data.Common.Extension;
using System.Data.JsonClient;
using System.Reflection;
using Xunit;

namespace Data.Json.Tests.FileAsDatabase;

public class JsonAlterTableTests
{
    [Fact]
    public void AddColumn_EmptyTable_ColumnAdded()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        AlterTableTests.AddColumn_EmptyTable_ColumnAdded(
                       () => new JsonConnection(ConnectionStrings.Instance.FileAsDB.Sandbox("Sandbox", sandboxId)));
    }

    [Fact]
    public void DropColumn_ColumnExists_Dropped()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        AlterTableTests.DropColumn_ColumnExists_Dropped(
                       () => new JsonConnection(ConnectionStrings.Instance.FileAsDB.Sandbox("Sandbox", sandboxId)));
    }
}
