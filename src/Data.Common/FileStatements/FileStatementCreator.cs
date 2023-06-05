using Data.Common.Parsing;
using Irony.Parsing;

namespace Data.Common.FileStatements;

internal class FileStatementCreator
{
    /// <summary>
    /// Supports scenario where EF Core calls FileCommand<TFileParameter>.ExecuteDbDataReader() with a multi-line
    /// SQL statement that first does an INSERT and then SELECTs the identity value of the newly INSERT'd row.
    /// </summary>
    /// <param name="fileCommand"></param>
    /// <returns></returns>
    public static IEnumerable<FileStatement> CreateMultiCommandSupport(DbCommand fileCommand)
    {
        var commandText = fileCommand.CommandText;
        var commands = commandText.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var command in commands)
        {
            var fileQuery = CreateFromCommand(command, fileCommand.Parameters);
            yield return fileQuery;
        }
    }

    public static FileStatement Create(IFileCommand fileCommand)
    {
        if (fileCommand.FileConnection.AdminMode)
            throw new ArgumentException($"The {nameof(FileStatement)}.{nameof(Create)} method cannot be used with an admin connection.");

        return CreateFromCommand(fileCommand.CommandText, fileCommand.Parameters as DbParameterCollection);
    }

    private static FileStatement CreateFromCommand(string commandText, DbParameterCollection parameters)
    {
        var parser = new Parser(new SqlGrammar());
        var parseTree = parser.Parse(commandText);
        if (parseTree.HasErrors())
        {
            ThrowHelper.ThrowQuerySyntaxException(string.Join(Environment.NewLine, parseTree.ParserMessages.Select(parserMessage => $"{parserMessage.Message}. Location={parserMessage.Location}")), commandText);
        }

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

        throw ThrowHelper.GetQueryNotSupportedException();
    }

}
