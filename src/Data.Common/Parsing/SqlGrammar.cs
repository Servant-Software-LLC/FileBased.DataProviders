using Irony.Parsing;
using SqlBuildingBlocks;

namespace Data.Common.Parsing;

// Loosely based on SQL89 grammar from Gold parser. Supports some extra TSQL constructs.
[Language("SQL", "89", "SQL 89 grammar")]
public class SqlGrammar : Grammar
{
    public SqlGrammar() : base(false) //SQL is case insensitive
    {
        Comment.Register(this);

        SimpleId simpleId = new(this);
        AliasOpt aliasOpt = new(simpleId, this);
        Id id = new(simpleId, this);
        IdList idList = new(id, this);
        TableName tableName = new(aliasOpt, id, simpleId, this);
        Expression expression = new();
        ExprList exprList = new(expression, this);
        FuncCall builtinFunc = new(id, exprList, this);
        Parameter parameter = new(this);
        JoinChainOpt joinChainOpt = new(tableName, expression, this);
        WhereClauseOpt whereClauseOpt = new(expression, this);
        OrderByList orderByList = new(id, this);
        SelectStmt selectStmt = new();

        expression.InitializeRule(selectStmt, id, exprList, builtinFunc, parameter, this);
        selectStmt.InitializeRule(id, idList, expression, exprList, builtinFunc, aliasOpt, tableName, joinChainOpt, whereClauseOpt, orderByList, this);

        InsertStmt insertStmt = new(id, idList, exprList, selectStmt, this);
        UpdateStmt updateStmt = new(id, expression, tableName, whereClauseOpt, this);
        DeleteStmt deleteStmt = new(tableName, whereClauseOpt, this);

        Stmt stmt = new(selectStmt, insertStmt, updateStmt, deleteStmt, this);

        Root = stmt;
    }

}
