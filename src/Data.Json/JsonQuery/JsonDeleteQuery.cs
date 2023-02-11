using Irony.Parsing;
namespace Data.Json.JsonQuery;

internal class JsonDeleteQuery : JsonQueryParser
{
    public JsonDeleteQuery(ParseTreeNode tree) : base(tree)
    {
    }

    public override IEnumerable<string> GetColumnNames()
    {
        throw new NotImplementedException();
    }

    public override string GetTable()
    {
        return node
         .ChildNodes[2].ChildNodes[0].Token.ValueString;

    }
}
