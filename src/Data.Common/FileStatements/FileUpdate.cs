using Irony.Parsing;
namespace Data.Common.FileStatements;

public class FileUpdate : FileStatement
{
    public FileUpdate(ParseTreeNode tree, DbParameterCollection parameters, string statement) 
        : base(tree, parameters, statement)
    {
    }

    public override IEnumerable<string> GetColumnNames()
    {
        var cols = node
     .ChildNodes[3].ChildNodes
     .Select(item => item.ChildNodes[0].ChildNodes[0].Token.ValueString);

        return cols;
    }

    public override SqlTable GetTable() => Parsing.TableName.Create(node.ChildNodes[1]);

    public IEnumerable<KeyValuePair<string, object>> GetValues()
    {
        var cols = GetColumnNames();
        var assignList = node.ChildNodes[3];
        var values = assignList.ChildNodes.Select(item => base.GetValue(item.ChildNodes[2]));
        
        if (cols.Count() != values.Count())
        {
            throw new InvalidOperationException("The supplied values are not matched");
        }

        var result = cols.Zip(values, (name, value) => KeyValuePair.Create(name, value));

        return result!;
    }
}
