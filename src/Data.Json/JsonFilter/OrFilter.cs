namespace Data.Json.JsonFilter;

public class OrFilter : Filter
{
    public Filter Left { get; set; }
    public Filter Right { get; set; }
    public OrFilter(Filter left, Filter right)
    {
        Left = left;
        Right = right;
    }
    public override string Evaluate()
    {
        return ToString();

    }
    public override string ToString()
    {
        return $"{Left.ToString()} OR {Right.ToString()}";
    }
}


