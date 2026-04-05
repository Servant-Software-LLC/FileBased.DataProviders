using Data.Tests.Common;
using System.Data.Common;
using System.Data.CsvClient;
using Xunit;

namespace Data.Csv.Tests.FolderAsDatabase;

public class CsvCommandBuilderTests
{
    [Fact]
    public void CreateCommandBuilder_ReturnsInstance()
    {
        CommandBuilderTests.CreateCommandBuilder_ReturnsInstance(CsvClientFactory.Instance);
    }

    [Fact]
    public void CreateCommandBuilder_HasCorrectQuoting()
    {
        CommandBuilderTests.CreateCommandBuilder_HasCorrectQuoting(CsvClientFactory.Instance);
    }

    [Fact]
    public void CreateCommandBuilder_QuoteIdentifier()
    {
        CommandBuilderTests.CreateCommandBuilder_QuoteIdentifier(CsvClientFactory.Instance);
    }

    [Fact]
    public void CreateCommandBuilder_UnquoteIdentifier()
    {
        CommandBuilderTests.CreateCommandBuilder_UnquoteIdentifier(CsvClientFactory.Instance);
    }

    [Fact]
    public void CommandBuilder_GetInsertCommand()
    {
        CommandBuilderTests.CommandBuilder_GetInsertCommand(
            () => new CsvConnection(ConnectionStrings.Instance.FolderAsDB),
            adapter => new CsvCommandBuilder((CsvDataAdapter)adapter));
    }

    [Fact]
    public void CommandBuilder_GetUpdateCommand_ThrowsWithoutKeyColumns()
    {
        CommandBuilderTests.CommandBuilder_GetUpdateCommand_ThrowsWithoutKeyColumns(
            () => new CsvConnection(ConnectionStrings.Instance.FolderAsDB),
            adapter => new CsvCommandBuilder((CsvDataAdapter)adapter));
    }

    [Fact]
    public void CommandBuilder_GetDeleteCommand_ThrowsWithoutKeyColumns()
    {
        CommandBuilderTests.CommandBuilder_GetDeleteCommand_ThrowsWithoutKeyColumns(
            () => new CsvConnection(ConnectionStrings.Instance.FolderAsDB),
            adapter => new CsvCommandBuilder((CsvDataAdapter)adapter));
    }
}
