namespace Data.Common.FileFilter;

public class OrFilter : Filter
{
    public Filter Left { get; set; }
    public Filter Right { get; set; }
    public OrFilter(Filter left, Filter right)
    {
        Left = left;
        Right = right;
    }
    public override string Evaluate() => ToString();
    public override string ToString() => $"{Left.Evaluate()} OR {Right.Evaluate()}";
}


