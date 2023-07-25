using Data.Common.Parsing;
using Data.Common.Utils;
using Irony.Parsing;
using Microsoft.Extensions.Logging;
using SqlBuildingBlocks;
using System.Text;

namespace Data.Common.FileStatements;

internal class FileStatementCreator
{
    /// <summary>
    /// Supports scenario where EF Core calls FileCommand<TFileParameter>.ExecuteDbDataReader() with a multi-line
    /// SQL statement that first does an INSERT and then SELECTs the identity value of the newly INSERT'd row.
    /// </summary>
    /// <param name="fileCommand"></param>
    /// <returns></returns>
    public static IList<FileStatement> CreateMultiCommandSupport(IFileCommand fileCommand, ILogger log)
    {
        var commandText = fileCommand.CommandText;
        var commands = commandText.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        return commands.Select(command => CreateFromCommand(command, fileCommand.FileConnection, fileCommand.Parameters, log)).ToList();
    }

    public static FileStatement Create(IFileCommand fileCommand, ILogger log)
    {
        if (fileCommand.FileConnection.AdminMode)
            throw new ArgumentException($"The {nameof(FileStatement)}.{nameof(Create)} method cannot be used with an admin connection.");

        return CreateFromCommand(fileCommand.CommandText, fileCommand.FileConnection, fileCommand.Parameters, log);
    }

    private static FileStatement CreateFromCommand(string commandText, IFileConnection fileConnection, DbParameterCollection parameters, ILogger log)
    {
        log.LogDebug($"FileStatementCreator.{nameof(CreateFromCommand)}() called.  CommandText = {commandText}");

        var grammar = new SqlGrammar();
        var parser = new Parser(grammar);
        var parseTree = parser.Parse(commandText);
        if (parseTree.HasErrors())
        {
            var errorMessage = string.Join(Environment.NewLine, parseTree.ParserMessages.Select(parserMessage => $"{parserMessage.Message}. Location={parserMessage.Location}"));
            log.LogError(errorMessage);
            ThrowHelper.ThrowQuerySyntaxException(errorMessage, commandText);
        }

        //The function provider isn't provided here (i.e. null), because it needs state that is provided to the Result class via its ctor in a previousWriteResult variable.
        var sqlDefinition = grammar.Create(parseTree.Root);
        if (log.IsEnabled(LogLevel.Debug))
        {
            log.LogDebug($"Parsed tree: {ParseTreeToString(parseTree.Root)}");
        }

        sqlDefinition.ResolveParameters(parameters);

        if (sqlDefinition.Insert != null)
            return new FileInsert(sqlDefinition.Insert, commandText);

        if (sqlDefinition.Delete != null)
            return new FileDelete(sqlDefinition.Delete, commandText);
            
        if (sqlDefinition.Update != null)
            return new FileUpdate(sqlDefinition.Update, commandText);

        if (sqlDefinition.Select != null)
        {
            if (sqlDefinition.Select.InvalidReferences)
                ThrowHelper.ThrowQuerySyntaxException(sqlDefinition.Select.InvalidReferenceReason, commandText);

            return new FileSelect(sqlDefinition.Select, commandText);
        }

        throw ThrowHelper.GetQueryNotSupportedException();
    }

    private static string ParseTreeToString(ParseTreeNode node, int indent = 0)
    {
        var sb = new StringBuilder();
        sb.AppendLine(new String(' ', indent * 2) + node.ToString());

        foreach (var child in node.ChildNodes)
        {
            sb.Append(ParseTreeToString(child, indent + 1));
        }

        return sb.ToString();
    }
}
