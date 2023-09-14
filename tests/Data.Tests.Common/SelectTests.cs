using Data.Common.Parsing;
using Data.Tests.Common.Utils;
using Xunit;

namespace Data.Tests.Common;

public class SelectTests
{
    [Fact]
    public void ctor_InterpretAstWithFunctions()
    {
        var grammar = new SqlGrammar();
        var commandText = "SELECT \"BlogId\"\r\nFROM \"Blogs\"\r\nWHERE ROW_COUNT() = 1\r\n AND \"BlogId\"=LAST_INSERT_ID()";
        var parseTreeNode = GrammarParser.Parse(grammar, commandText);

        var sqlDefinitions = grammar.Create(parseTreeNode).ToList();
        Assert.Single(sqlDefinitions);

        var sqlDefinition = sqlDefinitions[0];
        Assert.NotNull(sqlDefinition.Select);
        Assert.NotNull(sqlDefinition.Select!.WhereClause);

        var whereClause = sqlDefinition.Select!.WhereClause;
        var left = whereClause.Left;
        Assert.NotNull(left);
        Assert.NotNull(left.BinExpr);
        Assert.NotNull(left.BinExpr.Left.Function);
        var rowCountFunc = left.BinExpr.Left.Function;
        Assert.Equal("ROW_COUNT", rowCountFunc.FunctionName);

        var right = whereClause.Right;
        Assert.NotNull(right);
        Assert.NotNull(right.BinExpr);
        Assert.NotNull(right.BinExpr.Right.Function);
        var lastInsertIdFunc = right.BinExpr.Right.Function;
        Assert.Equal("LAST_INSERT_ID", lastInsertIdFunc.FunctionName);
    }
}
