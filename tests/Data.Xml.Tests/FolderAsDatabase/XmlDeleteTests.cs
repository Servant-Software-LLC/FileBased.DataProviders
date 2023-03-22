using Data.Tests.Common;
using System.Data.XmlClient;
using System.Reflection;
using Xunit;

namespace Data.Xml.Tests.FolderAsDatabase;

/// <summary>
/// These tests indirectly exercise the <see cref="JsonIO.Delete.JsonDelete"/> class via the <see cref="XmlCommand" calls to the different forms of the Execute methods/>. 
/// </summary>
public class XmlDeleteTests
{
    [Fact]
    public void Delete_ShouldDeleteData()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        DeleteTests.Delete_ShouldDeleteData(() => new XmlConnection(ConnectionStrings.Instance.FolderAsDBConnectionString.Sandbox("Sandbox", sandboxId)));
    }

}
