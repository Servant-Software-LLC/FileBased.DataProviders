using System.Text.RegularExpressions;

namespace Data.Xls.Utils;

public static class TypeGuesser
{
    public static Type GuessType(IEnumerable<string> columnValues)
    {
        Type previous = typeof(string);
        int num = 0;

        foreach (string columnValue in columnValues)
        {
            if (!string.Equals(columnValue, "null", StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrEmpty(columnValue))
            {
                previous = !bool.TryParse(columnValue, out bool _)
                    ? !float.TryParse(columnValue, out float _)
                        ? !DateTimeTryParse(columnValue)
                            ? DetermineType(num == 0, typeof(string), previous)
                            : DetermineType(num == 0, typeof(DateTime), previous)
                        : DetermineType(num == 0, typeof(float), previous)
                    : DetermineType(num == 0, typeof(bool), previous);
                ++num;
            }
        }

        return previous;
    }

    private static bool DateTimeTryParse(string columnValue)
    {
        // Ensures that the string begins with a date format where components are separated by - / or .
        const string datePattern = @"^(?:\d{1,4}[-/.]\d{1,2}[-/.]\d{1,4}).*$";

        if (DateTime.TryParse(columnValue, out DateTime _) &&
            Regex.IsMatch(columnValue, datePattern))
        {
            return true;
        }

        return false;
    }

    private static Type DetermineType(bool first, Type suggested, Type previous)
    {
        return first ? suggested : MaxKind(suggested, previous);
    }

    private static Type MaxKind(Type a, Type b)
    {
        if (a != b)
        {
            return typeof(string);
        }

        if (a == typeof(string) || b == typeof(string))
        {
            return typeof(string);
        }

        if (a == typeof(float) || b == typeof(float))
        {
            return typeof(float);
        }

        if (a == typeof(bool) || b == typeof(bool))
        {
            return typeof(bool);
        }

        return a == typeof(DateTime) || b == typeof(DateTime) ? typeof(DateTime) : typeof(string);
    }
}