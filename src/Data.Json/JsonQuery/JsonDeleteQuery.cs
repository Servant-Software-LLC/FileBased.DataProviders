using Irony.Parsing;
namespace Data.Json.JsonQuery;

internal class JsonDeleteQuery : JsonQuery
{
    public JsonDeleteQuery(ParseTreeNode tree, JsonCommand jsonCommand) : base(tree,jsonCommand)
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
