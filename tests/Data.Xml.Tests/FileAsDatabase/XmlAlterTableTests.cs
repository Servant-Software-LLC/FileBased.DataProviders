using Data.Tests.Common;
using Data.Common.Extension;
using System.Data.XmlClient;
using System.Reflection;
using Xunit;

namespace Data.Xml.Tests.FileAsDatabase;

public class XmlAlterTableTests
{
    [Fact]
    public void AddColumn_EmptyTable_ColumnAdded()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        AlterTableTests.AddColumn_EmptyTable_ColumnAdded(
                       () => new XmlConnection(ConnectionStrings.Instance.FileAsDB.Sandbox("Sandbox", sandboxId)));
    }

    [Fact]
    public void DropColumn_ColumnExists_Dropped()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        AlterTableTests.DropColumn_ColumnExists_Dropped(
                       () => new XmlConnection(ConnectionStrings.Instance.FileAsDB.Sandbox("Sandbox", sandboxId)));
    }
}
