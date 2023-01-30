﻿namespace Data.Json.JsonFilter;

public class AndAlsoFilter : Filter
{

    public Filter Left { get; set; }
    public Filter Right { get; set; }
    public AndAlsoFilter(Filter left, Filter right)
    {
        Left = left;
        Right = right;
    }
    public override string ToString()
    {
        return $"{Left.ToString()} AND ({Right.ToString()})";
    }
    public override string Evaluate()
    {
        return ToString();

    }
}

