using Irony.Parsing;
using Xunit;

namespace Data.Tests.Common.Utils;

public class GrammarParser
{
    public static ParseTreeNode Parse(Grammar grammar, string source)
    {
        ParseTree parseTree = ParseTree(grammar, source);

        Assert.False(parseTree.HasErrors(), string.Join(Environment.NewLine, parseTree.ParserMessages.Select(message => $"Location={message.Location} Message: {message.Message}")));

        return parseTree.Root;
    }

    public static ParseTree ParseTree(Grammar grammar, string source)
    {
        var language = new LanguageData(grammar);
        var fatalErrors = language.Errors.Where(err => !err.ToString().Contains("Shift-reduce conflict") || !err.ToString().Contains("Selected")).ToList();
        Assert.False(fatalErrors.Any(), string.Join(Environment.NewLine, fatalErrors.Select(err => err.ToString())));

        var parser = new Parser(language);
        var parseTree = parser.Parse(source);
        return parseTree;
    }
}
