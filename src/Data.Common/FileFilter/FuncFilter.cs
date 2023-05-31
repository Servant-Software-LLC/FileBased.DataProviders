using Data.Common.Utils;

namespace Data.Common.FileFilter;

class FuncFilter : Filter
{
    private readonly string funcName;
    private readonly string op;
    private readonly object value;
    private object funcValue;

    public FuncFilter(string funcName, string op, object value)
    {
        this.funcName = funcName;
        this.op = op;
        this.value = value;
    }

    public void EvaluateFunction(Result previousWriteResult) => funcValue = BuiltinFunction.EvaluateFunction(funcName, previousWriteResult);

    public override string ToString() => $"{funcName} {ChangeOp(op)} {value}";
    public override string Evaluate() => $"{funcValue} {ChangeOp(op)} {value}";

    public static string ChangeOp(string op)
    {
        return op switch
        {
            "!=" => "<>",
            _ => op
        };
    }
}
