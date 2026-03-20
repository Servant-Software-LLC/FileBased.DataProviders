using Data.Tests.Common;
using System.Data.XmlClient;
using Xunit;

namespace Data.Xml.Tests.FolderAsDatabase;

public class XmlClientFactoryTests
{
    [Fact]
    public void CreateCommand_ReadsData()
    {
        ClientFactoryTests.CreateCommand_ReadsData(XmlClientFactory.Instance);
    }

    [Fact]
    public void GetFactory_FromConnection_ReturnsXmlClientFactory()
    {
        using var connection = new XmlConnection();
        ClientFactoryTests.GetFactory_FromConnection_ReturnsSameInstance(connection, XmlClientFactory.Instance);
    }
}
