using Data.Common.FileJoin;
using Irony.Parsing;

namespace Data.Common.FileStatements;

public class FileSelect : FileStatement
{

    public FileSelect(ParseTreeNode tree, DbParameterCollection parameters, string statement)
        : base(tree, parameters, statement)
    {
    }

    public override IEnumerable<string> GetColumnNames()
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
                var colNode= x.ChildNodes[0].ChildNodes[0];

                if (colNode.Term.Name == "builtinFunc")
                {
                    col = colNode.ChildNodes[0].ChildNodes[0].Token.ValueString;
                }
                //Check If it is an aggregate query 
                else if (colNode.ChildNodes.Count > 1 && colNode.ChildNodes[0].Term.Name != "id_simple")
                {
                    var aggregateName = colNode.ChildNodes[0].ChildNodes[0].Token.ValueString;
                    ThrowHelper.ThrowIfNotSupportedAggregateFunctionException(aggregateName);
                    col = colNode.ChildNodes[1].ChildNodes[0].Token.ValueString;
                    ThrowHelper.ThrowIfNotAsterik(col);
                    IsCountQuery = true;
                }
                else
                {
                    var index = colNode.ChildNodes.Count == 2 ? 1 : 0;
                    col = colNode.ChildNodes[index].Token.ValueString;
                }

                return col;

            }).ToList();
        return cols;
    }

    public bool IsCountQuery { get; private set; }
    
    public override string GetTable()
    {
        var fromClauseOpt = node
          .ChildNodes
          .First(item => item.Term.Name == "fromClauseOpt");

        //If the columns are just constants or built-in functions, then it may not have a FROM clause
        //in the SELECT statement.
        if (fromClauseOpt.ChildNodes.Count == 0)
            return string.Empty;

        var tableId = fromClauseOpt.ChildNodes[1].ChildNodes[0];
        if (tableId.ChildNodes.Count == 2)
            return $"{tableId.ChildNodes[0].Token.ValueString}.{tableId.ChildNodes[1].Token.ValueString}";

        var table= tableId
        .ChildNodes[0]
        .Token
        .ValueString;

        return GetNameWithAlias(table).tableName;        
    }

    public DataTableJoin? GetFileJoin()
    {
        var mainTable = GetNameWithAlias(GetTable());
        var joinNode = node.ChildNodes[4].ChildNodes[2];
        if (joinNode.ChildNodes.Count==0)
            return null;

        var list = new List<Join>();
        AddJoin(list, joinNode);

        //remove aliases;
        foreach (var item in list)
        {
            RemoveAlias(item);
        }

        return new DataTableJoin(list, mainTable.tableName);
    }

    private void RemoveAlias(Join join)
    {
        join.TableName = GetNameWithAlias(join.TableName).tableName;
        join.SourceColumn = GetNameWithAlias(join.SourceColumn).tableName;
        join.JoinColumn = GetNameWithAlias(join.JoinColumn).tableName;
        foreach (var item in join.InnerJoin)
        {
            RemoveAlias(item);
        }
    }

    private void AddJoin(List<Join> list, ParseTreeNode joinNode)
    {
        var subNode = joinNode.ChildNodes[1];
        var table = subNode.ChildNodes[1].ChildNodes[0].ChildNodes[0].Token.ValueString;
        var tableAlias=GetNameWithAlias(table).alias;

        string sourceColumn = GetColumnWithAlias(subNode.ChildNodes[3]);
        string joinColumn = GetColumnWithAlias(subNode.ChildNodes[5]);
        if (!joinColumn.StartsWith(tableAlias)&& !sourceColumn.StartsWith(tableAlias))
        {
            ThrowHelper.ThrowQuerySyntaxException("Syntax error. Invalid ON join", $"SourceColumn = {sourceColumn} JoinColumn = {joinColumn}");
        }

        if (!joinColumn.StartsWith(tableAlias))
        {
            var join = joinColumn;
            joinColumn = sourceColumn;
            sourceColumn = join;
        }

        var fileJoin = new Join(table, joinColumn, sourceColumn);
        
        var hasAlreayJoin = FindJoin(list,x => GetNameWithAlias(x.TableName).alias == GetNameWithAlias(fileJoin.SourceColumn).alias);
        if (hasAlreayJoin!=null)
        {
            hasAlreayJoin.InnerJoin.Add(fileJoin);
        }
        else
        {
            list.Add(fileJoin);
        }

        if (joinNode.ChildNodes[2].ChildNodes.Count>0)
        {
            AddJoin(list,joinNode.ChildNodes[2]);
        }
    }

    private Join? FindJoin(List<Join> joins, Func<Join, bool> func)
    {
        foreach (var item in joins)
        {
            var join2 = FindJoinRecursive(item, func);
            if (join2 != null)
            {
                return join2;
            }
        }
        return null;
    }

    private Join? FindJoinRecursive(Join join,Func<Join, bool> func)
    {
        if (func(join))
        {
            return join;
        }
        foreach (var item in join.InnerJoin)
        {
            var join2 = FindJoinRecursive(item,func);
            if (join2!=null)
            {
                return join2;
            }
        }
        return null;
    }

    public static (string tableName, string alias) GetNameWithAlias(string name)
    {
        string alias = string.Empty;
        string tableName = name;
        if (!FileReader.IsSchemaTable(name) && !FileReader.IsSchemaColumn(name))
        {
            if (tableName.Contains(' '))
            {
                var data = tableName.Split(' ');
                if (data.Length > 1)
                {
                    tableName = data[0];
                    alias = data[1];
                }
            }
            else if (tableName.Contains('.'))
            {
                var data = tableName.Split('.');
                if (data.Length > 2)
                    throw new ArgumentException($"The identifier {name} has too many periods in it.");

                if (data.Length == 2)
                {
                    tableName = data[1];
                    alias = data[0];
                }
            }
        }

        return (tableName, alias);
    }

    private static string GetColumnWithAlias(ParseTreeNode sourceColumnNode) => 
        $"{sourceColumnNode.ChildNodes[0].Token.ValueString}.{sourceColumnNode.ChildNodes[1].Token.ValueString}";


}
