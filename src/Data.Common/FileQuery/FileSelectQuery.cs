using Data.Xml.JsonJoin;
using Irony.Parsing;

namespace Data.Common.FileQuery;

public class FileSelectQuery : FileQuery
{

    public FileSelectQuery(ParseTreeNode tree, FileCommand jsonCommand)
        : base(tree, jsonCommand)
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

                //Check If it is an aggregate query 
                if (colNode.ChildNodes.Count>1 && colNode.ChildNodes[0].Term.Name!= "id_simple")
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
        var table= fromClauseOpt
        .ChildNodes[1]
        .ChildNodes[0]
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
            ThrowHelper.ThrowSyntaxtErrorException("Invalid ON join");
        }
        if (!joinColumn.StartsWith(tableAlias))
        {
            var join = joinColumn;
            joinColumn = sourceColumn;
            sourceColumn = join;
        }
        var jsonJoin = new Join(table, joinColumn, sourceColumn);
        
        var hasAlreayJoin = FindJoin(list,x => GetNameWithAlias(x.TableName).alias == GetNameWithAlias(jsonJoin.SourceColumn).alias);
        if (hasAlreayJoin!=null)
        {
            hasAlreayJoin.InnerJoin.Add(jsonJoin);
        }
        else
        list.Add(jsonJoin);
        if (joinNode.ChildNodes[2].ChildNodes.Count>0)
        {
            AddJoin(list,joinNode.ChildNodes[2]);
        }
    }
    Join? FindJoin(List<Join> joins, Func<Join, bool> func)
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
    Join? FindJoinRecursive(Join join,Func<Join, bool> func)
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
    private static string GetColumnWithAlias(ParseTreeNode sourceColumnNode)
    {
        return $"{sourceColumnNode.ChildNodes[0].Token.ValueString}.{sourceColumnNode.ChildNodes[1].Token.ValueString}";
    }

    public (string tableName,string alias) GetNameWithAlias(string name)
    {
        if (name.Contains('.')||name.Contains(' '))
        {
            var data = name.Split(' ');

            if (data.Count() == 1)
            {
                data = name.Split('.');
                return (data[1], data[0]);

            }
            return (data[0], data[1]);
        }
        return (name,"");
    }
}
