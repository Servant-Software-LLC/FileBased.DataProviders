using Data.Tests.Common;
using System.Data.Common;
using System.Data.XmlClient;
using Xunit;

namespace Data.Xml.Tests.FolderAsDatabase;

public class XmlCommandBuilderTests
{
    [Fact]
    public void CreateCommandBuilder_ReturnsInstance()
    {
        CommandBuilderTests.CreateCommandBuilder_ReturnsInstance(XmlClientFactory.Instance);
    }

    [Fact]
    public void CreateCommandBuilder_HasCorrectQuoting()
    {
        CommandBuilderTests.CreateCommandBuilder_HasCorrectQuoting(XmlClientFactory.Instance);
    }

    [Fact]
    public void CreateCommandBuilder_QuoteIdentifier()
    {
        CommandBuilderTests.CreateCommandBuilder_QuoteIdentifier(XmlClientFactory.Instance);
    }

    [Fact]
    public void CreateCommandBuilder_UnquoteIdentifier()
    {
        CommandBuilderTests.CreateCommandBuilder_UnquoteIdentifier(XmlClientFactory.Instance);
    }

    [Fact]
    public void CommandBuilder_GetInsertCommand()
    {
        CommandBuilderTests.CommandBuilder_GetInsertCommand(
            () => new XmlConnection(ConnectionStrings.Instance.FolderAsDB),
            adapter => new XmlCommandBuilder((XmlDataAdapter)adapter));
    }

    [Fact]
    public void CommandBuilder_GetUpdateCommand_ThrowsWithoutKeyColumns()
    {
        CommandBuilderTests.CommandBuilder_GetUpdateCommand_ThrowsWithoutKeyColumns(
            () => new XmlConnection(ConnectionStrings.Instance.FolderAsDB),
            adapter => new XmlCommandBuilder((XmlDataAdapter)adapter));
    }

    [Fact]
    public void CommandBuilder_GetDeleteCommand_ThrowsWithoutKeyColumns()
    {
        CommandBuilderTests.CommandBuilder_GetDeleteCommand_ThrowsWithoutKeyColumns(
            () => new XmlConnection(ConnectionStrings.Instance.FolderAsDB),
            adapter => new XmlCommandBuilder((XmlDataAdapter)adapter));
    }
}
