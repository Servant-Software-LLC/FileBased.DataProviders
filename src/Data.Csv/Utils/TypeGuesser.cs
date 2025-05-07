using System.Text.RegularExpressions;

namespace Data.Csv.Utils;

// This class is a copy of DataFrame.DefaultGuessTypeFunction in the DataFrame library (https://learn.microsoft.com/en-us/dotnet/machine-learning/how-to-guides/getting-started-dataframe)
// with the following modifications to fix issues when uploading a CSV:
// 1. Adding a RegEx condition when parsing DateTime to avoid treating non-date strings such as "1 1" as DateTime object
// 2. Currently, if a column has mixture of data types that does not contain a string, it will guess the type depending on the priority given in MaxKind method.
// This can lead to incorrect format exceptions afterward when casting column values to the guessed data type. So, a condition is added in MaxKind method
// to set the guessed type as String if the current suggested and previous data type is not equal.
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