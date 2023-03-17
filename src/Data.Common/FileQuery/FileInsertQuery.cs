using Irony.Parsing;
namespace Data.Common.FileQuery;
public class FileInsertQuery : FileQuery
{
    public FileInsertQuery(ParseTreeNode node, FileCommand jsonCommand) : base(node, jsonCommand)
    {
    }

    public IEnumerable<KeyValuePair<string, object>> GetValues()
    {
        var cols = GetColumnNames();
        var values = node
            .ChildNodes[4]
            .ChildNodes[1]
            .ChildNodes.Select(x => x.ChildNodes.Count==0? x.Token.Value: base.GetValue(x.ChildNodes));
        if (cols.Count() != values.Count())
        {
            throw new InvalidOperationException("The supplied values are not matched");
        }

        var result = cols.Zip(values, (name, value) => KeyValuePair.Create(name, value));

        return result!;
    }

    public override IEnumerable<string> GetColumnNames()
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
