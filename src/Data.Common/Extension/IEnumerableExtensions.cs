namespace Data.Common.Extension;

public static class IEnumerableExtensions
{
    public static int IndexOf<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        int index = 0;
        foreach (var item in source)
        {
            if (predicate(item))
            {
                return index;
            }
            index++;
        }
        return -1; // Return -1 if no item is found
    }

#if NETSTANDARD2_0
    public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer = null)
    {
        return new HashSet<T>(source, comparer);
    }

    /// <summary>
    /// Returns the maximum value in a sequence according to a specified key selector function.
    /// This implementation is intended for use in target frameworks that do not support MaxBy natively.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of the input sequence.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by the keySelector function.</typeparam>
    /// <param name="source">An IEnumerable<T> to determine the maximum element of.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <returns>The value with the maximum key in the sequence.</returns>
    public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) where TKey : IComparable<TKey>
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));
        if (!source.Any()) throw new InvalidOperationException("Sequence contains no elements");

        TSource maxElement = source.First();
        TKey maxKey = keySelector(maxElement);

        foreach (TSource element in source.Skip(1))
        {
            TKey currentKey = keySelector(element);
            if (currentKey.CompareTo(maxKey) > 0)
            {
                maxElement = element;
                maxKey = currentKey;
            }
        }

        return maxElement;
    }

#endif
}
