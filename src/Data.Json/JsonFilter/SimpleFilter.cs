namespace Data.Json.JsonFilter;
public class SimpleFilter : Filter
{
    private readonly string propName;
    private readonly string op;
    private readonly object value;

    public SimpleFilter(string propName, string op, object value)
    {
        this.propName = propName;
        this.op = op;
        this.value = value;
    }
    public override string ToString()
    {
        return $"{propName} {ChangeOp(op)} '{value}'";
    }
    public override string Evaluate()
    {
        return ToString();
    }
    public string ChangeOp(string op)
    {
        return op switch
        {
            "!=" => "<>",
            _ => op
        };
    }

}


