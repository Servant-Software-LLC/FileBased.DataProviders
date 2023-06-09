using Irony.Parsing;
namespace Data.Common.FileStatements;

public class FileInsert : FileStatement
{
    public FileInsert(ParseTreeNode node, DbParameterCollection parameters, string statement) 
        : base(node, parameters, statement)
    {
    }

    public HashSet<string> ColumnNameHints { get; } = new();

    public IEnumerable<KeyValuePair<string, object>> GetValues()
    {
        var cols = GetColumnNames();
        var values = node
            .ChildNodes[4]
            .ChildNodes[1]
            .ChildNodes.Select(x => x.ChildNodes.Count==0? x.Token.Value: GetInsertParameterValue(x.ChildNodes));
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

    private object? GetInsertParameterValue(ParseTreeNodeList x)
    {
        if (x[0].Term.Name != "Parameter")
            throw new InvalidOperationException($"Expected all calls to GetInsertParameterValue() to be Parameter");

        return GetParameterValue(x[0].ChildNodes);
    }
}
