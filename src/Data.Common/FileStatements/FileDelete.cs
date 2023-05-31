using Irony.Parsing;

namespace Data.Common.FileStatements;

public class FileDelete : FileStatement
{
    public FileDelete(ParseTreeNode tree, DbParameterCollection parameters, string statement) 
        : base(tree, parameters, statement)
    {
    }

    public override IEnumerable<string> GetColumnNames()
    {
        throw new NotImplementedException();
    }
    public override string GetTable() => node.ChildNodes[2].ChildNodes[0].Token.ValueString;
}
