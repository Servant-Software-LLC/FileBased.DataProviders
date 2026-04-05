using Data.Tests.Common;
using System.Data.Common;
using System.Data.JsonClient;
using Xunit;

namespace Data.Json.Tests.FolderAsDatabase;

public class JsonCommandBuilderTests
{
    [Fact]
    public void CreateCommandBuilder_ReturnsInstance()
    {
        CommandBuilderTests.CreateCommandBuilder_ReturnsInstance(JsonClientFactory.Instance);
    }

    [Fact]
    public void CreateCommandBuilder_HasCorrectQuoting()
    {
        CommandBuilderTests.CreateCommandBuilder_HasCorrectQuoting(JsonClientFactory.Instance);
    }

    [Fact]
    public void CreateCommandBuilder_QuoteIdentifier()
    {
        CommandBuilderTests.CreateCommandBuilder_QuoteIdentifier(JsonClientFactory.Instance);
    }

    [Fact]
    public void CreateCommandBuilder_UnquoteIdentifier()
    {
        CommandBuilderTests.CreateCommandBuilder_UnquoteIdentifier(JsonClientFactory.Instance);
    }

    [Fact]
    public void CommandBuilder_GetInsertCommand()
    {
        CommandBuilderTests.CommandBuilder_GetInsertCommand(
            () => new JsonConnection(ConnectionStrings.Instance.FolderAsDB),
            adapter => new JsonCommandBuilder((JsonDataAdapter)adapter));
    }

    [Fact]
    public void CommandBuilder_GetUpdateCommand_ThrowsWithoutKeyColumns()
    {
        CommandBuilderTests.CommandBuilder_GetUpdateCommand_ThrowsWithoutKeyColumns(
            () => new JsonConnection(ConnectionStrings.Instance.FolderAsDB),
            adapter => new JsonCommandBuilder((JsonDataAdapter)adapter));
    }

    [Fact]
    public void CommandBuilder_GetDeleteCommand_ThrowsWithoutKeyColumns()
    {
        CommandBuilderTests.CommandBuilder_GetDeleteCommand_ThrowsWithoutKeyColumns(
            () => new JsonConnection(ConnectionStrings.Instance.FolderAsDB),
            adapter => new JsonCommandBuilder((JsonDataAdapter)adapter));
    }
}
