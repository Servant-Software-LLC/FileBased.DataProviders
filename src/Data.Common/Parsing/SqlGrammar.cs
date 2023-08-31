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

        SqlBuildingBlocks.Grammars.SQLServer.SimpleId simpleId = new(this);

        AliasOpt aliasOpt = new(simpleId, this);
        Id id = new(simpleId, this);
        LiteralValue literalValue = new();
        IdList idList = new(id, this);
        TableName tableName = new(aliasOpt, id, this);
        Expr expr = new();
        ExprList exprList = new(expr, this);
        FuncCall funcCall = new(id, exprList, this);
        Parameter parameter = new(this);
        JoinChainOpt joinChainOpt = new(tableName, expr, this);
        WhereClauseOpt whereClauseOpt = new(expr, this);
        OrderByList orderByList = new(id, this);
        SqlBuildingBlocks.Grammars.MySQL.SelectStmt selectStmt = new();

        expr.InitializeRule(selectStmt, id, literalValue, exprList, funcCall, parameter, this);
        selectStmt.InitializeRule(id, idList, expr, exprList, funcCall, aliasOpt, tableName, joinChainOpt, whereClauseOpt, orderByList, this);

        InsertStmt insertStmt = new(id, idList, exprList, selectStmt, this);
        UpdateStmt updateStmt = new(id, literalValue, parameter, funcCall, tableName, whereClauseOpt, this);
        DeleteStmt deleteStmt = new(tableName, whereClauseOpt, this);

        Stmt stmt = new(selectStmt, insertStmt, updateStmt, deleteStmt, this);
        Root = stmt;
    }

    public virtual SqlDefinition Create(ParseTreeNode selectStmt) =>
        ((Stmt)Root).Create(selectStmt);

}
