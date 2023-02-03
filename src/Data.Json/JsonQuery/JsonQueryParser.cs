using Irony.Parsing;
namespace Data.Json.JsonQuery
{
    internal abstract class JsonQueryParser
    {
        public string Table { get; set; }
        public Filter? Filter { get; set; }
        protected readonly ParseTreeNode node;
        public static JsonQueryParser Create(string query)
        {
            var parser = new Parser(new JsonGrammar());
            var parseTree = parser.Parse(query);
            if (parseTree.HasErrors())
            {
                ThrowHelper.ThrowSyntaxtErrorException(string.Join(Environment.NewLine, parseTree.ParserMessages));
            }
            var mainNode = parseTree.Root.ChildNodes[0];
            switch (mainNode.Term.Name)
            {
                case "insertStmt":
                    return new JsonInsertQuery(mainNode);
                case "deleteStmt":
                    return new JsonDeleteQuery(mainNode);
                case "updateStmt":
                    return new JsonUpdateQuery(mainNode);
                case "selectStmt":
                    return new JsonSelectQuery(mainNode);
                default:
                    ThrowHelper.ThrowQueryNotSupportedException();
                    break;
            }
            return default;
        }
        internal JsonQueryParser(ParseTreeNode node)
        {
            this.node = node;
            Filter = GetFilter();
            Table = GetTable();
        }
        public bool IsSelectQuery => GetType() == typeof(JsonSelectQuery);
        public bool IsInsertQuery => GetType() == typeof(JsonInsertQuery);
        public bool IsDeleteQuery => GetType() == typeof(JsonDeleteQuery);
        public bool IsUpdateQuery => GetType() == typeof(JsonUpdateQuery);
        public abstract string GetTable();
        public abstract IEnumerable<string> GetColumns();
        public virtual Filter? GetFilter()
        {
            var whereClause = node
                .ChildNodes
                .FirstOrDefault(item => item.Term.Name == "whereClauseOpt");
            if (whereClause?.ChildNodes.Count <= 0 || whereClause == null)
            {
                return null;
            }
            return ExtractFilter(whereClause!.ChildNodes[1].ChildNodes!);
        }
        protected Filter ExtractFilter(ParseTreeNodeList x)
        {
            Filter mainFilter = null;
            foreach (var item in x)
            {
                if (item.Term.Name == "binExpr")
                {
                    mainFilter = ExtractFilter(item.ChildNodes);
                }
                else if (item.Term.Name == "Id")
                {
                    var field = x[0].ChildNodes[0].Token.ValueString;
                    if (x[0].ChildNodes.Count > 1)
                        field += "." + x[0].ChildNodes[1].Token.ValueString;

                    var op = x[1].ChildNodes[0].Token.ValueString;
                    var value = x[2].Token.Value;

                    mainFilter = new SimpleFilter(field, op, value);
                    break;
                }
                else if (item.Term.Name == "binOp")
                {
                    var next = x[2];
                    var filter2 = ExtractFilter(next.ChildNodes);

                    var op = item.ChildNodes[0].Token.ValueString;
                    if (op.ToLower() == "and")
                    {
                        if (next.Term.Name == "binExpr")
                            mainFilter = Filter.And(mainFilter, filter2);
                        else
                            mainFilter = Filter.AndAlso(mainFilter, filter2);
                    }
                    else
                    {
                        if (next.Term.Name == "binExpr")
                            mainFilter = Filter.Or(mainFilter, filter2);
                        else
                            mainFilter = Filter.OrAlso(mainFilter, filter2);
                    }
                    break;
                }
            }
            return mainFilter;
        }


    }
}
