using Irony.Parsing;
namespace Data.Json.JsonQuery;

internal class JsonDeleteQuery : JsonQueryParser
{
    public JsonDeleteQuery(ParseTreeNode tree) : base(tree)
    {
        Table = GetTable();
    }
    public override IEnumerable<string> GetColumns()
    {
        throw new NotImplementedException();
    }
    public override string GetTable()
    {
        return node
         .ChildNodes[2].ChildNodes[0].Token.ValueString;

    }
}
