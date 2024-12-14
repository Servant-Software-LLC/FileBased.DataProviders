using Data.Common.Extension;
using Data.Tests.Common;
using System.Data.XmlClient;
using System.Reflection;
using Xunit;

namespace Data.Xml.Tests.FileAsDatabase;

public class XmlCreateTableTests
{
    [Fact]
    public void CreateTable_WhenNoColumns_ShouldWork()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        CreateTableTests.CreateTable_WhenNoColumns_ShouldWork(
                       () => new XmlConnection(ConnectionStrings.Instance.FileAsDB.Sandbox("Sandbox", sandboxId)));

    }

    [Fact]
    public void CreateTable_FollowedByInsert_ShouldWork()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        CreateTableTests.CreateTable_FollowedByInsert_ShouldWork(
                       () => new XmlConnection(ConnectionStrings.Instance.FileAsDB.Sandbox("Sandbox", sandboxId)));

    }
}
