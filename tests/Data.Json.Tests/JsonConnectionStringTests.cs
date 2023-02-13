using Data.Json.Utils.ConnectionString;
using Xunit;

namespace Data.Json.Tests;

public class JsonConnectionStringTests
{
    [Fact]
    public void ConnectionString_Empty()
    {
        JsonConnectionString connectionString = new();
        
        Assert.Throws<ArgumentNullException>("connectionString", () => connectionString.Parse(null));
    }

    [Fact]
    public void ConnectionString_ParsableButMissingDataSource()
    {
        JsonConnectionString connectionString = new();

        var exception = Assert.Throws<ArgumentException>("connectionString", () => connectionString.Parse("tree=oak;"));
        Assert.Contains(nameof(JsonConnectionStringKeywords.DataSource), exception.Message);
    }

    [Fact]
    public void ConnectionString_ParsableButContainsUnknownKeyword()
    {
        const string dataSourceValue = @"c:\database.json";
        const string unknownKeyword = "tree";
        const string connectionStringValue = $"DataSource={dataSourceValue};{unknownKeyword}=oak;";

        JsonConnectionString connectionString = new();

        var exception = Assert.Throws<ArgumentException>("connectionString", () => connectionString.Parse(connectionStringValue));
        Assert.Contains(unknownKeyword, exception.Message);
    }

    [Fact]
    public void ConnectionString_JustDataSource()
    {
        const string dataSourceValue = @"c:\database.json";
        const string connectionStringValue = $"DataSource={dataSourceValue};";

        JsonConnectionString connectionString = new();

        
        connectionString.Parse(connectionStringValue);

        Assert.Equal(connectionStringValue, connectionString.ConnectionString);
        Assert.Equal(dataSourceValue, connectionString.DataSource);
    }

    [Fact]
    public void ConnectionString_JustDataSourceAlias()
    {
        const string dataSourceValue = @"c:\database.json";
        const string connectionStringValue = $"Data Source={dataSourceValue};";

        JsonConnectionString connectionString = new();


        connectionString.Parse(connectionStringValue);

        Assert.Equal(connectionStringValue, connectionString.ConnectionString);
        Assert.Equal(dataSourceValue, connectionString.DataSource);
    }

}
