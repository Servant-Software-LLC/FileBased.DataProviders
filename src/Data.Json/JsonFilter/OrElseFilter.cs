namespace Data.Json.JsonFilter;
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
        return $"{Left.ToString()} || ({Right.ToString()})";
    }
    public override string Evaluate()
    {
        return ToString();

    }
}


