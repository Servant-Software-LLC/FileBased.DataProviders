using Data.Tests.Common;
using System.Data.XmlClient;
using System.Reflection;
using Xunit;

namespace Data.Xml.Tests.FileAsDatabase;

public class XmlDeleteTests
{
    [Fact]
    public void Delete_ShouldDeleteData()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        DeleteTests.Delete_ShouldDeleteData(() => new XmlConnection(ConnectionStrings.Instance.FileAsDBConnectionString.Sandbox("Sandbox", sandboxId)));
    }

}
