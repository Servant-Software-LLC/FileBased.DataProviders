using Data.Tests.Common;
using System.Data.XmlClient;
using System.Reflection;
using Xunit;

namespace Data.Xml.Tests.FolderAsDatabase;

/// <summary>
/// These tests indirectly exercise the <see cref="JsonIO.Write.JsonUpdate"/> class via calls to <see cref="XmlCommand.ExecuteNonQuery" />/>.
/// </summary>
public class XmlUpdateTests
{
    [Fact]
    public void Update_ShouldUpdateData()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        UpdateTests.Update_ShouldUpdateData(
            () => new XmlConnection(ConnectionStrings.Instance.FolderAsDBConnectionString.Sandbox("Sandbox", sandboxId))
        );
    }
}