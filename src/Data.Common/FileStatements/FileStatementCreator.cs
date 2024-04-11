using Data.Common.Parsing;
using Irony.Parsing;
using Microsoft.Extensions.Logging;
using SqlBuildingBlocks.LogicalEntities;
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

        return CreateFromCommand(commandText, fileCommand.FileConnection, fileCommand.Parameters, log).ToList();
    }

    public static FileStatement Create(IFileCommand fileCommand, ILogger log)
    {
        if (fileCommand.FileConnection.AdminMode)
            throw new ArgumentException($"The {nameof(FileStatement)}.{nameof(Create)} method cannot be used with an admin connection.");

        var fileStatements = CreateFromCommand(fileCommand.CommandText, fileCommand.FileConnection, fileCommand.Parameters, log).ToList();
        if (fileStatements.Count != 1)
            throw new ArgumentException($"The SQL '{fileCommand.CommandText}' did not yield 1 statement.  Count = {fileStatements.Count}");

        return fileStatements[0];
    }

    public static FileSelect CreateSelect(IFileCommand fileCommand, ILogger log)
    {
        if (fileCommand.FileConnection.AdminMode)
            throw new ArgumentException($"The {nameof(FileStatement)}.{nameof(CreateSelect)} method cannot be used with an admin connection.");

        var fileStatements = CreateSelectFromCommand(fileCommand.CommandText, fileCommand.FileConnection, fileCommand.Parameters, log).ToList();
        if (fileStatements.Count != 1)
            throw new ArgumentException($"The SQL '{fileCommand.CommandText}' did not yield 1 statement.  Count = {fileStatements.Count}");

        return fileStatements[0];
    }


    private static IEnumerable<FileSelect> CreateSelectFromCommand(string commandText, IFileConnection fileConnection, DbParameterCollection parameters, ILogger log)
    {
        log.LogDebug($"FileStatementCreator.{nameof(CreateFromCommand)}() called.  CommandText = {commandText}");

        var sqlDefinitions = CreateDefinitionsFromCommand(commandText, fileConnection, parameters, log);
        foreach (SqlDefinition sqlDefinition in sqlDefinitions)
        {
            sqlDefinition.ResolveParameters(parameters);

            if (sqlDefinition.Select != null)
            {
                if (sqlDefinition.Select.InvalidReferences)
                    ThrowHelper.ThrowQuerySyntaxException(sqlDefinition.Select.InvalidReferenceReason, commandText);

                yield return new FileSelect(sqlDefinition.Select, commandText);
                continue;
            }

            throw new Exception($"The command text, {commandText}, was not a SELECT statement.");
        }

    }


    private static IEnumerable<SqlDefinition> CreateDefinitionsFromCommand(string commandText, IFileConnection fileConnection, DbParameterCollection parameters, ILogger log)
    {
        log.LogDebug($"FileStatementCreator.{nameof(CreateDefinitionsFromCommand)}() called.  CommandText = {commandText}");

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
        var sqlDefinitions = grammar.Create(parseTree.Root);
        if (log.IsEnabled(LogLevel.Debug))
        {
            log.LogDebug($"Parsed tree: {ParseTreeToString(parseTree.Root)}");
        }

        return sqlDefinitions;
    }


    private static IEnumerable<FileStatement> CreateFromCommand(string commandText, IFileConnection fileConnection, DbParameterCollection parameters, ILogger log)
    {
        log.LogDebug($"FileStatementCreator.{nameof(CreateFromCommand)}() called.  CommandText = {commandText}");

        var sqlDefinitions = CreateDefinitionsFromCommand(commandText, fileConnection, parameters, log); 
        foreach (SqlDefinition sqlDefinition in sqlDefinitions)
        {
            sqlDefinition.ResolveParameters(parameters);

            if (sqlDefinition.Insert != null)
            {
                yield return new FileInsert(sqlDefinition.Insert, commandText);
                continue;
            }

            if (sqlDefinition.Delete != null)
            {
                yield return new FileDelete(sqlDefinition.Delete, commandText);
                continue;
            }

            if (sqlDefinition.Update != null)
            {
                yield return new FileUpdate(sqlDefinition.Update, commandText);
                continue;
            }

            if (sqlDefinition.Select != null)
            {
                if (sqlDefinition.Select.InvalidReferences)
                    ThrowHelper.ThrowQuerySyntaxException(sqlDefinition.Select.InvalidReferenceReason, commandText);

                yield return new FileSelect(sqlDefinition.Select, commandText);
                continue;
            }

            if (sqlDefinition.Create != null)
            {
                yield return new FileCreateTable(sqlDefinition.Create, commandText);
                continue;
            }

            if (sqlDefinition.Alter != null)
            {
                if (sqlDefinition.Alter.ColumnsToAdd.Count > 0)
                {
                    yield return new FileAddColumn(sqlDefinition.Alter, commandText);
                    continue;
                }

                if (sqlDefinition.Alter.ColumnsToDrop.Count > 0)
                {
                    yield return new FileDropColumn(sqlDefinition.Alter, commandText);
                    continue;
                }
            }

            throw ThrowHelper.GetQueryNotSupportedException();
        }

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
