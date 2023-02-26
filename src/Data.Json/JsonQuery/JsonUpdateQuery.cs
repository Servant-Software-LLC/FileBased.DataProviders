using Irony.Parsing;

namespace Data.Json.JsonQuery;

internal class JsonUpdateQuery : JsonQuery
{
    public JsonUpdateQuery(ParseTreeNode tree, JsonCommand jsonCommand) : base(tree, jsonCommand)
    {


    }

    public override IEnumerable<string> GetColumnNames()
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
        var cols = GetColumnNames();
        var values = node
     .ChildNodes[3].ChildNodes
     .Select(item =>  base.GetValue(item.ChildNodes));
        if (cols.Count() != values.Count())
        {
            throw new InvalidOperationException("The supplied values are not matched");
        }

        var result = cols.Zip(values, (name, value) => KeyValuePair.Create(name, value));

        return result;
    }
}
