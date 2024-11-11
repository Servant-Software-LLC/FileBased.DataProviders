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

}
