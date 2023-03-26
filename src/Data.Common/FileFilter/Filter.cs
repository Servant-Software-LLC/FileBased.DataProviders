namespace Data.Common.FileFilter;

public abstract class Filter
{
    public static Filter Create(string propName, string op, object value) => 
        new SimpleFilter(propName, op, value);

    public static Filter And(Filter left, Filter right) => new AndFilter(left, right);
    public static Filter AndAlso(Filter left, Filter right) => new AndAlsoFilter(left, right);
    public static Filter OrAlso(Filter left, Filter right) => new OrElseFilter(left, right);
    public static Filter Or(Filter left, Filter right) => new OrFilter(left, right);

    public abstract string Evaluate();
}


