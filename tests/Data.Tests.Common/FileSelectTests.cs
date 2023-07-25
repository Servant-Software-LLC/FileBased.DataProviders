using Data.Common.FileStatements;
using Data.Common.Parsing;
using Irony.Parsing;
using SqlBuildingBlocks.LogicalEntities;
using System.Linq.Expressions;
using Xunit;

namespace Data.Tests.Common;

public class FileSelectTests
{
    [Fact]
    public void ctor_InterpretAstWithFunctions()
    {
        var commandText = "SELECT \"BlogId\"\r\nFROM \"Blogs\"\r\nWHERE ROW_COUNT() = 1\r\n AND \"BlogId\"=LAST_INSERT_ID()";
        var grammar = new SqlGrammar();
        var parser = new Parser(grammar);
        var parseTree = parser.Parse(commandText);
        Assert.False(parseTree.HasErrors());

        var sqlDefinition = grammar.Create(parseTree.Root);

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
