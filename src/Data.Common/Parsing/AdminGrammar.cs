using Irony.Parsing;

namespace Data.Common.Parsing;

// Loosely based on SQL89 grammar from Gold parser. Supports some extra TSQL constructs.
[Language("SQL", "89", "SQL 89 grammar")]
public class AdminGrammar : Grammar
{
    public AdminGrammar() : base(false)
    { //SQL is case insensitive
      //Terminals
        var comment = new CommentTerminal("comment", "/*", "*/");
        var lineComment = new CommentTerminal("line_comment", "--", "\n", "\r\n");
        NonGrammarTerminals.Add(comment);
        NonGrammarTerminals.Add(lineComment);

        var FilePath = new StringLiteral("FilePath", "'", StringOptions.AllowsDoubledQuote | StringOptions.NoEscapes);

        var createDatabaseStmt = new NonTerminal("createDatabaseStmt");
        createDatabaseStmt.Rule = ToTerm("CREATE DATABASE") + FilePath;

        var dropDatabaseStmt = new NonTerminal("dropDatabaseStmt");
        dropDatabaseStmt.Rule = ToTerm("DROP DATABASE") + FilePath;

        this.Root = new NonTerminal("Root");
        this.Root.Rule = createDatabaseStmt | dropDatabaseStmt;

        MarkPunctuation("CREATE DATABASE", "DROP DATABASE", "\"");

    }//constructor

}//class
//namespace