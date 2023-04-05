using Data.Tests.Common;
using System.Data.XmlClient;
using System.Reflection;
using Xunit;

namespace Data.Xml.Tests.FileAsDatabase;

public class XmlInsertTests
{
    [Fact]
    public void Insert_ShouldInsertData()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        InsertTests.Insert_ShouldInsertData(() => new XmlConnection(ConnectionStrings.Instance.FileAsDBConnectionString.Sandbox("Sandbox", sandboxId)));
    }

    [Fact]
    public void Insert_JsonShouldBeFormatted()
    {
        InsertTests.Insert_ShouldBeFormattedForFile(() =>
        new XmlConnection(ConnectionStrings.Instance
        .FileAsDBConnectionString.AddFormatted(true)));
    }
}