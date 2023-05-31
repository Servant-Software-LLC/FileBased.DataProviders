using Data.Common.Utils.ConnectionString;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Data.Xml.Tests;

public class XmlConnectionStringTests
{
    [Fact]
    public void ConnectionString_Empty()
    {
        FileConnectionString connectionString = new();
        
        Assert.Throws<ArgumentNullException>("connectionString", () => connectionString.ConnectionString = null);
    }

    [Fact]
    public void ConnectionString_ParsableButMissingDataSource()
    {
        FileConnectionString connectionString = new();

        var exception = Assert.Throws<ArgumentException>("connectionString", () => connectionString = "tree=oak;");
        Assert.Contains(nameof(FileConnectionStringKeywords.DataSource), exception.Message);
    }

    [Fact]
    public void ConnectionString_ParsableButContainsUnknownKeyword()
    {
        const string dataSourceValue = @"c:\database.xml";
        const string unknownKeyword = "tree";
        const string connectionStringValue = $"DataSource={dataSourceValue};{unknownKeyword}=oak;";

        FileConnectionString connectionString = new();

        var exception = Assert.Throws<ArgumentException>("connectionString", () => connectionString = connectionStringValue);
        Assert.Contains(unknownKeyword, exception.Message);
    }

    [Fact]
    public void ConnectionString_JustDataSource()
    {
        const string dataSourceValue = @"c:\database.xml";
        const string connectionStringValue = $"DataSource={dataSourceValue};";

        FileConnectionString connectionString = new();

        
        connectionString = connectionStringValue;

        Assert.Equal(connectionStringValue, connectionString.ConnectionString);
        Assert.Equal(dataSourceValue, connectionString.DataSource);
    }

    [Fact]
    public void ConnectionString_JustDataSourceAlias()
    {
        const string dataSourceValue = @"c:\database.xml";
        const string connectionStringValue = $"Data Source={dataSourceValue};";

        FileConnectionString connectionString = new();


        connectionString = connectionStringValue;

        Assert.Equal($"DataSource={dataSourceValue};", connectionString.ConnectionString);
        Assert.Equal(dataSourceValue, connectionString.DataSource);
    }

    [Fact]
    public void ConnectionString_LogLevel()
    {
        const string dataSourceValue = @"c:\database.xml";
        const string connectionStringValue = $"Data Source={dataSourceValue};LogLevel=Debug";

        FileConnectionString connectionString = connectionStringValue;

        Assert.Equal($"DataSource={dataSourceValue};LogLevel=Debug;", connectionString.ConnectionString);
        Assert.Equal(LogLevel.Debug, connectionString.LogLevel);
    }

    [Fact]
    public void ConnectionString_LogLevel_InvalidValue()
    {
        const string dataSourceValue = @"c:\database.xml";
        const string connectionStringValue = $"Data Source={dataSourceValue};LogLevel=Shutdown";

        FileConnectionString connectionString = new();

        var exception = Assert.Throws<ArgumentException>("connectionString", () => connectionString = connectionStringValue);
        Assert.Contains("LogLevel", exception.Message);
    }

}
