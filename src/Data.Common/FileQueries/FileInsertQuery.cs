using Irony.Parsing;
namespace Data.Common.FileQueries;

public class FileInsertQuery : FileQuery
{
    public FileInsertQuery(ParseTreeNode node, DbParameterCollection parameters) 
        : base(node, parameters)
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

    public override string GetTable() => node.ChildNodes[2].ChildNodes[0].Token.ValueString;
}
