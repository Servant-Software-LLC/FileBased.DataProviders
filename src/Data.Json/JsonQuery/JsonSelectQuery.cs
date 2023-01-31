using Irony.Parsing;

namespace Data.Json.JsonQuery
{
    internal class JsonSelectQuery : JsonQueryParser
    {

        public JsonSelectQuery(ParseTreeNode tree) : base(tree)
        {
        }

        public override IEnumerable<string> GetColumns()
        {
            var colListNode = node
            .ChildNodes
            .First(item => item.Term.Name == "selList" && item.ChildNodes.Count > 0)
            .ChildNodes[0];
            var col = colListNode.Token?.ValueString;
            if (col == "*")
            {
                return new List<string>()
                {
                    col
                };
            }
            var cols = colListNode
                .ChildNodes[0]
                .ChildNodes[0]
                .ChildNodes
                .Select(x => x.ChildNodes[0].Token.ValueString).ToList();
            return cols;
        }


        public override string GetTable()
        {
            var fromClauseOpt = node
              .ChildNodes
              .First(item => item.Term.Name == SqlKeywords.FromClauseOpt);
            return fromClauseOpt
            .ChildNodes[1]
            .ChildNodes[0]
            .ChildNodes[0]
            .Token
            .ValueString;
        }
    }
}
