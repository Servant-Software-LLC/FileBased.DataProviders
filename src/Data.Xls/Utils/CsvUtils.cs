using Data.Csv.Utils;

namespace Data.Xls.Utils;

public static class CsvUtils
{
    public static IEnumerable<string> EscapeCsvValues(this IEnumerable<string> values)
    {
        foreach (var value in values)
        {
            // A whitespace-only cell would be trimmed to an empty string by DataFrame's CSV reader
            // (TextFieldParser.TrimWhiteSpace). Wrap it in a guard character on both ends so the field's
            // edges are non-whitespace and survive parsing; CsvVirtualDataTable strips the guard afterwards.
            var guarded = value.Length > 0 && string.IsNullOrWhiteSpace(value)
                ? CsvVirtualDataTable.WhitespaceGuard + value + CsvVirtualDataTable.WhitespaceGuard
                : value;

            if (guarded.Contains(",") || guarded.Contains("\""))
            {
                yield return $"\"{guarded.Replace("\"", "\"\"")}\"";
            }
            else
            {
                yield return guarded;
            }
        }
    }
}