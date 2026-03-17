using Data.Csv.Utils;
using Data.Tests.Common;
using System.Data.CsvClient;
using Xunit;

namespace Data.Csv.Tests.FolderAsDatabase;

public class CsvConnectionStringBuilderTests
{
    [Fact]
    public void ConnectionStringBuilder_CanBuildAndParse()
    {
        ConnectionStringBuilderTests.ConnectionStringBuilder_CanBuildAndParse(
            new CsvConnectionStringBuilder(),
            "/path/to/folder");
    }

    [Fact]
    public void Factory_CreateConnectionStringBuilder_ReturnsCsvBuilder()
    {
        var factory = CsvClientFactory.Instance;
        var builder = factory.CreateConnectionStringBuilder();
        Assert.IsType<CsvConnectionStringBuilder>(builder);
    }
}
