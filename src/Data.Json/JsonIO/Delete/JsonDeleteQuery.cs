using Irony.Parsing;
namespace Data.Json.JsonIO.Delete
{
    internal class JsonDeleteQuery : JsonQueryParser
    {
        public JsonDeleteQuery(ParseTreeNode tree) : base(tree)
        {
            this.Table = GetTable();
        }

        public override IEnumerable<string> GetColumns()
        {
            throw new NotImplementedException();
        }

        //public override Filter? GetFilter()
        //{
        //    throw new NotImplementedException();
        //}

        public override string GetTable()
        {
            return node
             .ChildNodes[2].ChildNodes[0].Token.ValueString;

        }
    }
}
