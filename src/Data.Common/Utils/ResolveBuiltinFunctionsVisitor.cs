using SqlBuildingBlocks.LogicalEntities;
using SqlBuildingBlocks.Visitors;

namespace Data.Common.Utils;

internal class ResolveBuiltinFunctionsVisitor : ResolveFunctionsVisitor
{
    private readonly Result previousWriteResult;

    public ResolveBuiltinFunctionsVisitor(Result previousWriteResult)
    {
        this.previousWriteResult = previousWriteResult ?? throw new ArgumentNullException(nameof(previousWriteResult));    
    }

    public override SqlExpression Visit(SqlFunction function)
    {
        var value = BuiltinFunction.EvaluateFunction(function.FunctionName, previousWriteResult);

        if (value != null) 
        {
            var sqlLiteralValue = new SqlLiteralValue(value);
            return new SqlExpression(sqlLiteralValue);
        }

        throw new Exception($"The value for function {function} was null.");
    }
}
