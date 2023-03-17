namespace Data.Common.FileFilter;
public class OrElseFilter : Filter
{
    public Filter Left { get; set; }
    public Filter Right { get; set; }
    public OrElseFilter(Filter left, Filter right)
    {
        Left = left;
        Right = right;
    }
    public override string ToString()
    {
        return $"{Left.Evaluate()} || ({Right.Evaluate()})";
    }
    public override string Evaluate()
    {
        return ToString();

    }
}


