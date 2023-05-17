using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Linq.Expressions;

namespace EFCore.Csv.Query.Internal;

internal class CsvQuerySqlGenerator : QuerySqlGenerator
{
    public CsvQuerySqlGenerator(QuerySqlGeneratorDependencies dependencies)
        : base(dependencies)
    {
    }

    protected override string GetOperator(SqlBinaryExpression binaryExpression)
    {
        if (binaryExpression == null)
            throw new ArgumentNullException(nameof(binaryExpression));

        return binaryExpression.OperatorType == ExpressionType.Add
            && binaryExpression.Type == typeof(string)
                ? " || "
                : base.GetOperator(binaryExpression);
    }

    protected override void GenerateLimitOffset(SelectExpression selectExpression)
    {
        if (selectExpression == null)
            throw new ArgumentNullException(nameof(selectExpression));

        if (selectExpression.Limit != null
            || selectExpression.Offset != null)
        {
            Sql.AppendLine()
                .Append("LIMIT ");

            Visit(
                selectExpression.Limit
                ?? new SqlConstantExpression(Expression.Constant(-1), selectExpression.Offset!.TypeMapping));

            if (selectExpression.Offset != null)
            {
                Sql.Append(" OFFSET ");

                Visit(selectExpression.Offset);
            }
        }
    }

    protected override void GenerateSetOperationOperand(SetOperationBase setOperation, SelectExpression operand)
    {
        if (setOperation == null)
            throw new ArgumentNullException(nameof(setOperation));
        if (operand == null)
            throw new ArgumentNullException(nameof(operand));

        // Sqlite doesn't support parentheses around set operation operands
        Visit(operand);
    }

}
