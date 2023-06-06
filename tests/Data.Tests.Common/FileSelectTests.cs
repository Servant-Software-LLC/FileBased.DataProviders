using Data.Common.FileStatements;
using Data.Common.Parsing;
using Irony.Parsing;
using System.Data.FileClient;
using Xunit;

namespace Data.Tests.Common;

public class FileSelectTests
{
    private class DummyParameter : FileParameter<DummyParameter>
    {
        public override DummyParameter Clone() => throw new NotImplementedException();
    }

    [Fact]
    public void ctor_InterpretAstWithFunctions()
    {
        var commandText = "SELECT \"BlogId\"\r\nFROM \"Blogs\"\r\nWHERE ROW_COUNT() = 1\r\n AND \"BlogId\"=LAST_INSERT_ID()";
        var parser = new Parser(new SqlGrammar());
        var parseTree = parser.Parse(commandText);
        Assert.False(parseTree.HasErrors());

        var mainNode = parseTree.Root.ChildNodes[0];
        Assert.Equal("selectStmt", mainNode.Term.Name);
        var parameters = new FileParameterCollection<DummyParameter>();
        var select = new FileSelect(mainNode, parameters, commandText);


    }
}
