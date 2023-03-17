namespace Data.Common.Extension;

internal static class ObjectExtensions
{
    internal static TTarget Convert<TTarget>(this object source) => (TTarget)source;
}
