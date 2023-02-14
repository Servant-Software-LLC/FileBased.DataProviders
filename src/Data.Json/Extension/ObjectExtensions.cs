namespace Data.Json.Extension;

internal static class ObjectExtensions
{
    internal static TTarget Convert<TTarget>(this object source) => (TTarget)source;
}
