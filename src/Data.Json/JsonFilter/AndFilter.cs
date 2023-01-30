﻿namespace Data.Json.JsonFilter;

public class AndFilter : Filter
{

    public Filter Left { get; set; }
    public Filter Right { get; set; }
    public AndFilter(Filter left, Filter right)
    {
        Left = left;
        Right = right;
    }
    public override string ToString()
    {
        return $"{Left.ToString()} AND {Right.ToString()}";
    }
    public override string Evaluate()
    {
        return ToString();
    }
}

