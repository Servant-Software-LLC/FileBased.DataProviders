using Data.Tests.Common;
using System.Data.CsvClient;
using Xunit;

namespace Data.Csv.Tests.FolderAsDatabase;

public class CsvClientFactoryTests
{
    [Fact]
    public void CreateCommand_ReadsData()
    {
        ClientFactoryTests.CreateCommand_ReadsData(CsvClientFactory.Instance);
    }
}
