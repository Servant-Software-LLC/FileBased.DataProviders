using Data.Json.JsonJoin;
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
                .ChildNodes.Select(x =>
                {
                    string col = string.Empty;
                   var colNode= x.ChildNodes[0]
                    .ChildNodes[0];
                    //Check If it is an aggregate query
                    if (colNode.ChildNodes.Count>1)
                    {
                        var aggregateName = colNode.ChildNodes[0].ChildNodes[0].Token.ValueString;
                        ThrowHelper.ThrowIfNotSupportedAggregateFunctionException(aggregateName);
                        col = colNode.ChildNodes[1].ChildNodes[0].Token.ValueString;
                        ThrowHelper.ThrowIfNotAsterik(col);
                        IsCountQuery = true;
                    }
                    else
                    {
                        col = colNode.ChildNodes[0].Token.ValueString;
                    }
                    return col;

                }).ToList();
            return cols;
        }
        public JsonJoin.DataTableJoin? Join { get; set; }
        public bool IsCountQuery { get; set; }
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
        public JsonJoin.DataTableJoin? GetJsonJoin()
        {
            return default;
        }
    }
}
