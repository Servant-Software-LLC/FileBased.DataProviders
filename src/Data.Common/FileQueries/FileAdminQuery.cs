using Data.Common.Parsing;
using Irony.Parsing;

namespace Data.Common.FileQueries;

public abstract class FileAdminQuery<TFileParameter>
    where TFileParameter : FileParameter<TFileParameter>, new()
{
    protected readonly ParseTreeNode node;
    protected readonly FileCommand<TFileParameter> fileCommand;

    public FileAdminQuery(ParseTreeNode node, FileCommand<TFileParameter> fileCommand)
    {
        this.node = node;
        this.fileCommand = fileCommand;
    }

    public string Database { get; private set; }


    /// <summary>
    /// 
    /// </summary>
    /// <returns>True if operation was performed on object of interest.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public abstract bool Execute();

    public static FileAdminQuery<TFileParameter> Create(FileCommand<TFileParameter> fileCommand)
    {
        if (!fileCommand.FileConnection.AdminMode)
            throw new ArgumentException($"The {nameof(FileAdminQuery<TFileParameter>)}.{nameof(Create)} method can only be used with an admin connection.");

        var parser = new Parser(new AdminGrammar());
        var parseTree = parser.Parse(fileCommand.CommandText);
        if (parseTree.HasErrors())
        {
            ThrowHelper.ThrowQuerySyntaxException(string.Join(Environment.NewLine, parseTree.ParserMessages), fileCommand.CommandText);
        }

        var mainNode = parseTree.Root.ChildNodes[0];
        FileAdminQuery<TFileParameter> command;
        switch (mainNode.Term.Name)
        {
            case "createDatabaseStmt":
                command = new FileCreateDatabaseQuery<TFileParameter>(mainNode, fileCommand);
                break;
            case "dropDatabaseStmt":
                command = new FileDropDatabaseQuery<TFileParameter>(mainNode, fileCommand);
                break;
            default:
                throw ThrowHelper.GetQueryNotSupportedException();
        }

        //Get the database file path.
        command.Database = mainNode.ChildNodes[0].Token.ValueString;

        return command;        
    }
}