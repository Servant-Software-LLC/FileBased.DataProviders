using Irony.Parsing;
namespace Data.Common.FileQuery;

public class FileUpdateQuery<TFileParameter> : FileQuery<TFileParameter>
    where TFileParameter : FileParameter<TFileParameter>, new()
{
    public FileUpdateQuery(ParseTreeNode tree, FileCommand<TFileParameter> fileCommand) : base(tree, fileCommand)
    {
    }

    public override IEnumerable<string> GetColumnNames()
    {
        var cols = node
     .ChildNodes[3].ChildNodes
     .Select(item => item.ChildNodes[0].ChildNodes[0].Token.ValueString);

        return cols;
    }

    public override string GetTable() => node.ChildNodes[1].ChildNodes[0].Token.ValueString;

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

        return result!;
    }
}
