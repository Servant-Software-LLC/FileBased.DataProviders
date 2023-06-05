using Data.Common.Parsing;
using Irony.Parsing;
using Microsoft.Extensions.Logging;
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
    public static IList<FileStatement> CreateMultiCommandSupport(DbCommand fileCommand, ILogger log)
    {
        var commandText = fileCommand.CommandText;
        var commands = commandText.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        return commands.Select(command => CreateFromCommand(command, fileCommand.Parameters, log)).ToList();
    }

    public static FileStatement Create(IFileCommand fileCommand, ILogger log)
    {
        if (fileCommand.FileConnection.AdminMode)
            throw new ArgumentException($"The {nameof(FileStatement)}.{nameof(Create)} method cannot be used with an admin connection.");

        return CreateFromCommand(fileCommand.CommandText, fileCommand.Parameters as DbParameterCollection, log);
    }

    private static FileStatement CreateFromCommand(string commandText, DbParameterCollection parameters, ILogger log)
    {
        log.LogDebug($"FileStatementCreator.{nameof(CreateFromCommand)}() called.  CommandText = {commandText}");

        var parser = new Parser(new SqlGrammar());
        var parseTree = parser.Parse(commandText);
        if (parseTree.HasErrors())
        {
            var errorMessage = string.Join(Environment.NewLine, parseTree.ParserMessages.Select(parserMessage => $"{parserMessage.Message}. Location={parserMessage.Location}"));
            log.LogError(errorMessage);
            ThrowHelper.ThrowQuerySyntaxException(errorMessage, commandText);
        }


        if (log.IsEnabled(LogLevel.Debug))
        {
            log.LogDebug($"Parsed tree: {ParseTreeToString(parseTree.Root)}");
        }

        //Catch any exceptions, since ASTs can vary a lot, these classes can end up throwing many exceptions
        //in seldomly tested scenarios and when the grammar changes.
        try
        {
            var mainNode = parseTree.Root.ChildNodes[0];
            switch (mainNode.Term.Name)
            {
                case "insertStmt":
                    return new FileInsert(mainNode, parameters, commandText);
                case "deleteStmt":
                    return new FileDelete(mainNode, parameters, commandText);
                case "updateStmt":
                    return new FileUpdate(mainNode, parameters, commandText);
                case "selectStmt":
                    return new FileSelect(mainNode, parameters, commandText);
            }
        }
        catch (Exception ex)
        {
            log.LogError($"Error interpreting AST: {ex}");
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
