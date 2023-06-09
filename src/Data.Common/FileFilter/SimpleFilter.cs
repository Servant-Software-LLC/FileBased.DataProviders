using Data.Common.Utils;

namespace Data.Common.FileFilter;

public class SimpleFilter : Filter
{
    private readonly object leftValue;
    private readonly string op;
    private readonly object rightValue;

    public SimpleFilter(object leftValue, string op, object rightValue)
    {
        this.leftValue = leftValue;
        this.op = op;
        this.rightValue = rightValue;
    }

    public override string ToString() => $"{ToStringValue(leftValue)} {ChangeOp(op)} {ToStringValue(rightValue)}";
    public override string Evaluate() => $"{ToEvaluateValue(leftValue)} {ChangeOp(op)} {ToEvaluateValue(rightValue)}";

    internal override void ResolveFunctions(Result previousWriteResult)
    {
        if (leftValue is Func leftFuncValue)
        {
            leftFuncValue.ResolveFunctionValue(previousWriteResult);
            ContainsBuiltinFunction = true;
        }

        if (rightValue is Func rightFuncValue)
        {
            rightFuncValue.ResolveFunctionValue(previousWriteResult);
            ContainsBuiltinFunction = true;
        }
    }

    private static string ToStringValue(object value) => value switch
    {
        Field field => field.ToString(),
        Func func => func.Name.ToString(),
        _ => $"'{value}'"
    };

    private static string ToEvaluateValue(object value) => value switch
    {
        Field field => field.ToString(),
        Func func => func.Value.ToString(),
        string str => $"'{value}'",
        _ => $"{value}"
    };

    public static string ChangeOp(string op)
    {
        return op switch
        {
            "!=" => "<>",
            _ => op
        };
    }

}


