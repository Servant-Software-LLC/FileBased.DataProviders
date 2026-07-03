using Data.Common.Utils;

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
            //
            // Scope: only *fully* whitespace-only cells are guarded. Leading/trailing whitespace on
            // otherwise-non-empty text (e.g. " x ") is still trimmed by the reader and is not preserved.
            var guarded = value.Length > 0 && string.IsNullOrWhiteSpace(value)
                ? CsvWhitespaceGuard.Sentinel + value + CsvWhitespaceGuard.Sentinel
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