using Data.Common.Utils;

namespace Data.Common.FileFilter;

public class AndAlsoFilter : Filter
{
    public Filter Left { get; set; }
    public Filter Right { get; set; }

    public AndAlsoFilter(Filter left, Filter right)
    {
        Left = left;
        Right = right;
    }

    public override string ToString() => $"{Left.Evaluate()} AND ({Right.Evaluate()})";
    public override string Evaluate() => ToString();

    internal override void ResolveFunctions(Result previousWriteResult)
    {
        Left.ResolveFunctions(previousWriteResult);
        Right.ResolveFunctions(previousWriteResult);

        if ((Left.ContainsBuiltinFunction.HasValue && Left.ContainsBuiltinFunction.Value) || 
            (Right.ContainsBuiltinFunction.HasValue && Right.ContainsBuiltinFunction.Value))
            ContainsBuiltinFunction = true;
    }
}


