using Data.Common.Utils.ConnectionString;
using Xunit;

namespace Data.Csv.Tests;

public class CsvConnectionStringTests
{
    [Fact]
    public void ConnectionString_Empty()
    {
        FileConnectionString connectionString = new();
        
        Assert.Throws<ArgumentNullException>("connectionString", () => connectionString.Parse(null));
    }

    [Fact]
    public void ConnectionString_ParsableButMissingDataSource()
    {
        FileConnectionString connectionString = new();

        var exception = Assert.Throws<ArgumentException>("connectionString", () => connectionString.Parse("tree=oak;"));
        Assert.Contains(nameof(FileConnectionStringKeywords.DataSource), exception.Message);
    }

    [Fact]
    public void ConnectionString_ParsableButContainsUnknownKeyword()
    {
        const string dataSourceValue = @"c:\database.csv";
        const string unknownKeyword = "tree";
        const string connectionStringValue = $"DataSource={dataSourceValue};{unknownKeyword}=oak;";

        FileConnectionString connectionString = new();

        var exception = Assert.Throws<ArgumentException>("connectionString", () => connectionString.Parse(connectionStringValue));
        Assert.Contains(unknownKeyword, exception.Message);
    }

    [Fact]
    public void ConnectionString_JustDataSource()
    {
        const string dataSourceValue = @"c:\database.csv";
        const string connectionStringValue = $"DataSource={dataSourceValue};";

        FileConnectionString connectionString = new();

        
        connectionString.Parse(connectionStringValue);

        Assert.Equal(connectionStringValue, connectionString.ConnectionString);
        Assert.Equal(dataSourceValue, connectionString.DataSource);
    }

    [Fact]
    public void ConnectionString_JustDataSourceAlias()
    {
        const string dataSourceValue = @"c:\database.csv";
        const string connectionStringValue = $"Data Source={dataSourceValue};";

        FileConnectionString connectionString = new();


        connectionString.Parse(connectionStringValue);

        Assert.Equal(connectionStringValue, connectionString.ConnectionString);
        Assert.Equal(dataSourceValue, connectionString.DataSource);
    }

}
