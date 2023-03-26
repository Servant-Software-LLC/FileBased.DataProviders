namespace Data.Common.FileFilter;
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

    public override string ToString() => $"{propName} {ChangeOp(op)} '{value}'";
    public override string Evaluate() => ToString();

    public static string ChangeOp(string op)
    {
        return op switch
        {
            "!=" => "<>",
            _ => op
        };
    }

}


