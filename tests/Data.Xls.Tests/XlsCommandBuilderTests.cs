using Data.Tests.Common;
using System.Data.XlsClient;
using Xunit;

namespace Data.Xls.Tests;

public class XlsCommandBuilderTests
{
    [Fact]
    public void CreateCommandBuilder_ReturnsInstance()
    {
        CommandBuilderTests.CreateCommandBuilder_ReturnsInstance(XlsClientFactory.Instance);
    }

    [Fact]
    public void CreateCommandBuilder_HasCorrectQuoting()
    {
        CommandBuilderTests.CreateCommandBuilder_HasCorrectQuoting(XlsClientFactory.Instance);
    }

    [Fact]
    public void CreateCommandBuilder_QuoteIdentifier()
    {
        CommandBuilderTests.CreateCommandBuilder_QuoteIdentifier(XlsClientFactory.Instance);
    }

    [Fact]
    public void CreateCommandBuilder_UnquoteIdentifier()
    {
        CommandBuilderTests.CreateCommandBuilder_UnquoteIdentifier(XlsClientFactory.Instance);
    }
}
