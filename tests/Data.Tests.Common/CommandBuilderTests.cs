using System.Data;
using System.Data.Common;
using System.Data.FileClient;
using Xunit;

namespace Data.Tests.Common;

public static class CommandBuilderTests
{
    public static void CreateCommandBuilder_ReturnsInstance(DbProviderFactory dbProviderFactory)
    {
        // Act
        var commandBuilder = dbProviderFactory.CreateCommandBuilder();

        // Assert
        Assert.NotNull(commandBuilder);
        Assert.IsAssignableFrom<DbCommandBuilder>(commandBuilder);
    }

    public static void CreateCommandBuilder_HasCorrectQuoting(DbProviderFactory dbProviderFactory)
    {
        // Act
        var commandBuilder = dbProviderFactory.CreateCommandBuilder();

        // Assert
        Assert.Equal("[", commandBuilder!.QuotePrefix);
        Assert.Equal("]", commandBuilder!.QuoteSuffix);
    }

    public static void CreateCommandBuilder_QuoteIdentifier(DbProviderFactory dbProviderFactory)
    {
        // Arrange
        var commandBuilder = dbProviderFactory.CreateCommandBuilder();

        // Act
        var quoted = commandBuilder!.QuoteIdentifier("employees");

        // Assert
        Assert.Equal("[employees]", quoted);
    }

    public static void CreateCommandBuilder_UnquoteIdentifier(DbProviderFactory dbProviderFactory)
    {
        // Arrange
        var commandBuilder = dbProviderFactory.CreateCommandBuilder();

        // Act
        var unquoted = commandBuilder!.UnquoteIdentifier("[employees]");

        // Assert
        Assert.Equal("employees", unquoted);
    }

    public static void CommandBuilder_GetInsertCommand<TFileParameter>(
        Func<FileConnection<TFileParameter>> createFileConnection,
        Func<DbDataAdapter, DbCommandBuilder> createCommandBuilder)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        // Arrange
        var connection = createFileConnection();
        connection.Open();

        var selectCommand = connection.CreateCommand("SELECT * FROM employees");
        var adapter = selectCommand.CreateAdapter();

        var dataSet = new DataSet();
        adapter.Fill(dataSet);

        var commandBuilder = createCommandBuilder(adapter);

        // Act
        var insertCommand = commandBuilder.GetInsertCommand();

        // Assert
        Assert.NotNull(insertCommand);
        Assert.False(string.IsNullOrEmpty(insertCommand.CommandText));
        Assert.True(insertCommand.Parameters.Count > 0);

        connection.Close();
    }

    public static void CommandBuilder_GetUpdateCommand_ThrowsWithoutKeyColumns<TFileParameter>(
        Func<FileConnection<TFileParameter>> createFileConnection,
        Func<DbDataAdapter, DbCommandBuilder> createCommandBuilder)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        // Arrange
        var connection = createFileConnection();
        connection.Open();

        var selectCommand = connection.CreateCommand("SELECT * FROM employees");
        var adapter = selectCommand.CreateAdapter();

        var dataSet = new DataSet();
        adapter.Fill(dataSet);

        var commandBuilder = createCommandBuilder(adapter);

        // Act & Assert
        // File-based providers do not expose key column information,
        // so dynamic SQL generation for UpdateCommand is not supported.
        Assert.Throws<InvalidOperationException>(() => commandBuilder.GetUpdateCommand());

        connection.Close();
    }

    public static void CommandBuilder_GetDeleteCommand_ThrowsWithoutKeyColumns<TFileParameter>(
        Func<FileConnection<TFileParameter>> createFileConnection,
        Func<DbDataAdapter, DbCommandBuilder> createCommandBuilder)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        // Arrange
        var connection = createFileConnection();
        connection.Open();

        var selectCommand = connection.CreateCommand("SELECT * FROM employees");
        var adapter = selectCommand.CreateAdapter();

        var dataSet = new DataSet();
        adapter.Fill(dataSet);

        var commandBuilder = createCommandBuilder(adapter);

        // Act & Assert
        // File-based providers do not expose key column information,
        // so dynamic SQL generation for DeleteCommand is not supported.
        Assert.Throws<InvalidOperationException>(() => commandBuilder.GetDeleteCommand());

        connection.Close();
    }
}
