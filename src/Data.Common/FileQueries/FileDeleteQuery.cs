using Irony.Parsing;

namespace Data.Common.FileQueries;

public class FileDeleteQuery : FileQuery
{
    public FileDeleteQuery(ParseTreeNode tree, DbParameterCollection parameters) 
        : base(tree, parameters)
    {
    }

    public override IEnumerable<string> GetColumnNames()
    {
        throw new NotImplementedException();
    }
    public override string GetTable() => node.ChildNodes[2].ChildNodes[0].Token.ValueString;
}
