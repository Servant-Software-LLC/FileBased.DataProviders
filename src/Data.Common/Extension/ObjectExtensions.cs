namespace Data.Common.Extension;

public static class ObjectExtensions
{
    public static TTarget Convert<TTarget>(this object source) => (TTarget)source;

    public static T GetValueAsType<T>(this object obj)
    {
        return (T)System.Convert.ChangeType(obj, typeof(T))!;
    }
}
