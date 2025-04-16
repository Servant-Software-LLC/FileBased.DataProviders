using Data.Tests.Common;
using System.Data.JsonClient;
using Xunit;

namespace Data.Json.Tests.FolderAsDatabase;

public class JsonClientFactoryTests
{
    [Fact]
    public void CreateCommand_ReadsData()
    {
        ClientFactoryTests.CreateCommand_ReadsData(JsonClientFactory.Instance);
    }

}
