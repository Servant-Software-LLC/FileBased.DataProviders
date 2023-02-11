using Irony.Parsing;

namespace Data.Json.JsonQuery;

internal class JsonInsertQuery : JsonQueryParser
{
    public JsonInsertQuery(ParseTreeNode node) : base(node)
    {
    }

    public IEnumerable<KeyValuePair<string, object>> GetValues()
    {
        var cols = GetColumns();
        var values = node
            .ChildNodes[4]
            .ChildNodes[1]
            .ChildNodes.Select(x => x.Token.Value);
        if (cols.Count() != values.Count())
        {
            throw new InvalidOperationException("The supplied values are not matched");
        }

        var result = cols.Zip(values, (name, value) => KeyValuePair.Create(name, value));

        return result;
    }

    public override IEnumerable<string> GetColumns()
    {
        var cols = node
       .ChildNodes[3].ChildNodes[0].ChildNodes
       .Select(item => item.ChildNodes[0].Token.ValueString);
        return cols;
    }
    public override string GetTable()
    {
        return node.ChildNodes[2].ChildNodes[0].Token.ValueString;
    }
}
