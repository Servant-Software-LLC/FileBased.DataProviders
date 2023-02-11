using Irony.Parsing;

namespace Data.Json.JsonQuery;

internal class JsonUpdateQuery : JsonQueryParser
{
    public JsonUpdateQuery(ParseTreeNode tree) : base(tree)
    {


    }

    public override IEnumerable<string> GetColumns()
    {
        var cols = node
     .ChildNodes[3].ChildNodes
     .Select(item => item.ChildNodes[0].ChildNodes[0].Token.ValueString);

        return cols;
    }



    public override string GetTable()
    {
        return node.ChildNodes[1].ChildNodes[0].Token.ValueString;
    }

    public IEnumerable<KeyValuePair<string, object>> GetValues()
    {
        var cols = GetColumns();
        var values = node
     .ChildNodes[3].ChildNodes
     .Select(item => item.ChildNodes[2].Token.Value);
        if (cols.Count() != values.Count())
        {
            throw new InvalidOperationException("The supplied values are not matched");
        }

        var result = cols.Zip(values, (name, value) => KeyValuePair.Create(name, value));

        return result;
    }
}
