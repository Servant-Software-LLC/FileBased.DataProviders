using Data.Tests.Common;
using Data.Common.Extension;
using System.Data.CsvClient;
using System.Reflection;
using Xunit;

namespace Data.Csv.Tests.FolderAsDatabase;

public class CsvAlterTableTests
{
    [Fact]
    public void AddColumn_EmptyTable_ColumnAdded()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        AlterTableTests.AddColumn_EmptyTable_ColumnAdded(
                       () => new CsvConnection(ConnectionStrings.Instance.FolderAsDB.Sandbox("Sandbox", sandboxId)));
    }

    [Fact]
    public void DropColumn_ColumnExists_Dropped()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        AlterTableTests.DropColumn_ColumnExists_Dropped(
                       () => new CsvConnection(ConnectionStrings.Instance.FolderAsDB.Sandbox("Sandbox", sandboxId)));
    }
}
