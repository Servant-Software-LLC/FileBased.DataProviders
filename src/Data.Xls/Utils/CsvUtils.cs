namespace Data.Xls.Utils;

public static class CsvUtils
{
    public static IEnumerable<string> EscapeCsvValues(this IEnumerable<string> values)
    {
        foreach (var value in values)
        {
            if (value.Contains(",") || value.Contains("\""))
            {
                yield return $"\"{value.Replace("\"", "\"\"")}\"";
            }
            else
            {
                yield return value;
            }
        }
    }
}