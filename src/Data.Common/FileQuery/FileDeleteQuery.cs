using Irony.Parsing;
namespace Data.Common.FileQuery;
public class FileDeleteQuery : FileQuery
{
    public FileDeleteQuery(ParseTreeNode tree, FileCommand jsonCommand) : base(tree,jsonCommand)
    {
    }
    public override IEnumerable<string> GetColumnNames()
    {
        throw new NotImplementedException();
    }
    public override string GetTable()
    {
        return node
         .ChildNodes[2].ChildNodes[0].Token.ValueString;

    }
}
