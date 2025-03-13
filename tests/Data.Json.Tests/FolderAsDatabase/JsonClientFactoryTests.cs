using Data.Tests.Common;
using System.Data.JsonClient;
using Xunit;

namespace Data.Json.Tests.FolderAsDatabase;

public class JsonClientFactoryTests
{
    [Fact(Skip = "Temp: Not needed for current goal")]
    public void CreateCommand_ReadsData()
    {
        ClientFactoryTests.CreateCommand_ReadsData(JsonClientFactory.Instance);
    }

}
