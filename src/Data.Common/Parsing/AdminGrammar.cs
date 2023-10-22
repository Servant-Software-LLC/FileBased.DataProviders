using Irony.Parsing;
using SqlBuildingBlocks;

namespace Data.Common.Parsing;

// Loosely based on SQL89 grammar from Gold parser. Supports some extra TSQL constructs.
[Language("SQL", "89", "SQL 89 grammar")]
public class AdminGrammar : Grammar
{
    public AdminGrammar() : base(false)
    { //SQL is case insensitive

        Comment.Register(this);

        var FilePath = new StringLiteral("FilePath", "'", StringOptions.AllowsDoubledQuote | StringOptions.NoEscapes);

        var createDatabaseStmt = new NonTerminal("createDatabaseStmt");
        createDatabaseStmt.Rule = ToTerm("CREATE DATABASE") + FilePath;

        var dropDatabaseStmt = new NonTerminal("dropDatabaseStmt");
        dropDatabaseStmt.Rule = ToTerm("DROP DATABASE") + FilePath;

        Root = new NonTerminal("Root");
        Root.Rule = createDatabaseStmt | dropDatabaseStmt;

        MarkPunctuation("CREATE DATABASE", "DROP DATABASE", "\"");

    }//constructor

}//class
//namespace