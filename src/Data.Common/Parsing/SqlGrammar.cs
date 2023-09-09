using Irony.Parsing;
using SqlBuildingBlocks;
using SqlBuildingBlocks.LogicalEntities;

namespace Data.Common.Parsing;

// Loosely based on SQL89 grammar from Gold parser. Supports some extra TSQL constructs.
[Language("SQL", "89", "SQL 89 grammar")]
public class SqlGrammar : Grammar
{
    public SqlGrammar() : base(false) //SQL is case insensitive
    {
        Comment.Register(this);

        //NOTE: Using SQL Server's naming scheme.
        SqlBuildingBlocks.Grammars.SQLServer.SimpleId simpleId = new(this);
        Id id = new(this, simpleId);

        //NOTE: Using SQL Server's data types.
        SqlBuildingBlocks.Grammars.SQLServer.DataType dataType = new(this);

        //NOTE: Using MySQL's LIMIT and OFFSET clauses
        SqlBuildingBlocks.Grammars.MySQL.SelectStmt selectStmt = new(this, id);

        selectStmt.Expr.InitializeRule(selectStmt, selectStmt.FuncCall);

        InsertStmt insertStmt = new(this, selectStmt);
        UpdateStmt updateStmt = new(this, selectStmt);
        DeleteStmt deleteStmt = new(this, selectStmt);
        CreateTableStmt createTableStmt = new(this, id, dataType);

        Stmt stmt = new(this, selectStmt, insertStmt, updateStmt, deleteStmt, createTableStmt);
        StmtLine stmtLine = new(this, stmt);

        Root = stmtLine;
    }

    public virtual IEnumerable<SqlDefinition> Create(ParseTreeNode stmtList) =>
        ((StmtLine)Root).Create(stmtList);

}
