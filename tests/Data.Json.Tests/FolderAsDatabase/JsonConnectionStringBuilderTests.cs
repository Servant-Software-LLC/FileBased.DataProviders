using Data.Json.Utils;
using Data.Tests.Common;
using System.Data.JsonClient;
using Xunit;

namespace Data.Json.Tests.FolderAsDatabase;

public class JsonConnectionStringBuilderTests
{
    [Fact]
    public void ConnectionStringBuilder_CanBuildAndParse()
    {
        ConnectionStringBuilderTests.ConnectionStringBuilder_CanBuildAndParse(
            new JsonConnectionStringBuilder(),
            "/path/to/database.json");
    }

    [Fact]
    public void JsonConnectionStringBuilder_Formatted_Property()
    {
        var builder = new JsonConnectionStringBuilder();
        builder.DataSource = "/path/to/database.json";
        builder.Formatted = true;

        var cs = builder.ConnectionString;
        var builder2 = new JsonConnectionStringBuilder();
        builder2.ConnectionString = cs;

        Assert.Equal(true, builder2.Formatted);
    }

    [Fact]
    public void Factory_CreateConnectionStringBuilder_ReturnsJsonBuilder()
    {
        var factory = JsonClientFactory.Instance;
        var builder = factory.CreateConnectionStringBuilder();
        Assert.IsType<JsonConnectionStringBuilder>(builder);
    }
}
