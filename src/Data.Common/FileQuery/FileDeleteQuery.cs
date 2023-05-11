using Irony.Parsing;
namespace Data.Common.FileQuery;
public class FileDeleteQuery<TFileParameter> : FileQuery<TFileParameter>
    where TFileParameter : FileParameter<TFileParameter>, new()
{
    public FileDeleteQuery(ParseTreeNode tree, FileCommand<TFileParameter> fileCommand) : base(tree,fileCommand)
    {
    }
    public override IEnumerable<string> GetColumnNames()
    {
        throw new NotImplementedException();
    }
    public override string GetTable() => node.ChildNodes[2].ChildNodes[0].Token.ValueString;
}
