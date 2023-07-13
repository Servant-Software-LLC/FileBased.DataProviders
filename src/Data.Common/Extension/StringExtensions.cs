namespace Data.Common.Extension;

internal static class StringExtensions
{
    public static string CamelCase(this string str)
    {
        if (string.IsNullOrEmpty(str))
            return str;

        return char.ToLower(str[0]) + str.Substring(1);
    }
}
