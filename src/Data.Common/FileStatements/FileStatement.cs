using Data.Common.Parsing;
using Irony.Parsing;
namespace Data.Common.FileStatements;

public abstract class FileStatement
{
    protected readonly ParseTreeNode node;
    private readonly DbParameterCollection parameters;

    protected FileStatement(ParseTreeNode node, DbParameterCollection parameters, string statement)
    {
        this.node = node;
        this.parameters = parameters;
        Filter = GetFilters();
        TableName = GetTable();
        Statement = statement;
    }

    public string TableName { get; }
    public Filter? Filter { get; }

    /// <summary>
    /// SQL statement that created this instance.
    /// </summary>
    public string Statement { get; }

    public abstract string GetTable();
    public abstract IEnumerable<string> GetColumnNames();
    public virtual Filter? GetFilters()
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

    protected Filter? ExtractFilter(ParseTreeNodeList x)
    {
        Filter? mainFilter = null;
        foreach (var item in x)
        {
            if (item.Term.Name == "binExpr")
            {
                mainFilter = ExtractFilter(item.ChildNodes);
            }
            else if (item.Term.Name == "Id" || item.Term.Name == "builtinFunc" || item.Term.Name == "number" || item.Term.Name == "string")
            {
                object? leftValue = GetValue(x[0]);

                var op = x[1].ChildNodes[0].Token.ValueString;

                //check if the query is parameterized
                object? rightValue = GetValue(x[2]);

                mainFilter = new SimpleFilter(leftValue, op, rightValue!);
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
                        mainFilter = Filter.And(mainFilter!, filter2!);
                    else
                        mainFilter = Filter.AndAlso(mainFilter!, filter2!);
                }
                else
                {
                    if (next.Term.Name == "binExpr")
                        mainFilter = Filter.Or(mainFilter!, filter2!);
                    else
                        mainFilter = Filter.OrAlso(mainFilter!, filter2!);
                }
                break;
            }
        }
        return mainFilter;
    }

    protected object? GetValue(ParseTreeNode valueNode)
    {
        object? value = string.Empty;
        if (valueNode.Term.Name.StartsWith("Unnamed"))
        {
            value = GetParameterValue(valueNode.ChildNodes);
        }
        else if (valueNode.Term.Name == "Id")
        {
            var fieldName = valueNode.ChildNodes[0].Token.ValueString;
            if (valueNode.ChildNodes.Count > 1)
                fieldName += "." + valueNode.ChildNodes[1].Token.ValueString;

            value = new Field(fieldName);
        }
        else if (valueNode.Term.Name == "builtinFunc")
        {
            var funcName = valueNode.ChildNodes[0].ChildNodes[0].Token.ValueString;

            value = new Func(funcName);
        }
        else
            value = valueNode.Token.Value;

        return value;
    }

    protected object? GetParameterValue(ParseTreeNodeList x)
    {
        object? value;
        string paramName = GetParameterName(x);
        if (!parameters.Contains(paramName))
        {
            throw new InvalidOperationException($"Must declare the scalar variable \"@{paramName}\"");
        }

        var parameter = parameters[paramName].Convert<IDbDataParameter>();
        value = parameter.Value;
        return value;

    }

    private string GetParameterName(ParseTreeNodeList x)
    {
        if (x[0].ChildNodes.Count == 0)
        {
            return x[0].Token.ValueString;
        }
        return x[0].ChildNodes[0].Token.ValueString;
    }
}
