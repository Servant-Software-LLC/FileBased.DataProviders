namespace Data.Json.Extension;

internal static class StringExtensions
{
    internal static TTarget Convert<TTarget>(this string source) => (TTarget)System.Convert.ChangeType(source, typeof(TTarget));
}
