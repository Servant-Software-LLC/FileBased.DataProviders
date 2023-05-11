using System.Globalization;

namespace System.Data.FileClient;


public class FileParameterCollection<TFileParameter> : DbParameterCollection, IDataParameterCollection, IList<TFileParameter>
    where TFileParameter : FileParameter<TFileParameter>, new()
{
    internal List<TFileParameter> InternalList { get; } = new(5);

    public override bool Contains(string parameterName) => -1 != IndexOf(parameterName);

    /// <inheritdoc />
    public override bool Contains(object value)
        => value is TFileParameter param && InternalList.Contains(param);

    /// <summary>
    /// Report whether the specified parameter is present in the collection.
    /// </summary>
    /// <param name="item">Parameter to find.</param>
    /// <returns>True if the parameter was found, otherwise false.</returns>
    public bool Contains(TFileParameter item) => InternalList.Contains(item);

    public override int IndexOf(string parameterName)
    {
        int index = 0;
        foreach (TFileParameter item in this)
        {
            if (0 == CultureAwareCompare(item.ParameterName, parameterName))
            {
                return index;
            }
            index++;
        }
        return -1;
    }

    IEnumerator<TFileParameter> IEnumerable<TFileParameter>.GetEnumerator()
        => InternalList.GetEnumerator();

    /// <inheritdoc />
    public override IEnumerator GetEnumerator() => InternalList.GetEnumerator();

    /// <summary>
    /// Gets the number of <see cref="TFileParameter"/> objects in the collection.
    /// </summary>
    /// <value>The number of <see cref="TFileParameter"/> objects in the collection.</value>
    public override int Count => InternalList.Count;

    /// <summary>
    /// Report the offset within the collection of the given parameter.
    /// </summary>
    /// <param name="item">Parameter to find.</param>
    /// <returns>Index of the parameter, or -1 if the parameter is not present.</returns>
    public int IndexOf(TFileParameter item)
        => InternalList.IndexOf(item);

    /// <inheritdoc />
    public override int IndexOf(object value)
        => IndexOf(Cast(value));

    /// <summary>
    /// Removes the specified <see cref="TFileParameter"/> from the collection.
    /// </summary>
    /// <param name="value">The <see cref="TFileParameter"/> to remove from the collection.</param>
    public override void Remove(object value)
        => InternalList.Remove(Cast(value));

    /// <summary>
    /// Remove the specified parameter from the collection.
    /// </summary>
    /// <param name="item">Parameter to remove.</param>
    /// <returns>True if the parameter was found and removed, otherwise false.</returns>
    public bool Remove(TFileParameter item) => InternalList.Remove(item);

    /// <inheritdoc />
    public override void RemoveAt(string parameterName)
        => RemoveAt(IndexOf(parameterName ?? throw new ArgumentNullException(nameof(parameterName))));

    /// <summary>
    /// Removes the specified <see cref="NpgsqlParameter"/> from the collection using a specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the parameter.</param>
    public override void RemoveAt(int index)
    {
        if (InternalList.Count - 1 < index)
            throw new ArgumentOutOfRangeException(nameof(index));

        Remove(InternalList[index]);
    }

    /// <inheritdoc />
    void ICollection<TFileParameter>.Add(TFileParameter item)
        => Add(item);

    public override int Add(object? value)
    {
        if (value is TFileParameter fileParameter)
            return Add(fileParameter);

        return Add(new TFileParameter { Value = value });
    }

    public int Add(TFileParameter value)
    {
        if (string.IsNullOrEmpty(value.ParameterName))
            throw new ArgumentException($"{nameof(value.ParameterName)} property of the {nameof(TFileParameter)} must be named");

        InternalList.Add(value);
        return Count - 1;
    }

    public int Add(string parameterName, DbType type) => Add(new TFileParameter{ ParameterName = parameterName, DbType = type } );

    public int Add(string parameterName, object value) => Add(new TFileParameter { ParameterName = parameterName, Value = value });

    public int Add(string parameterName, DbType dbType, string sourceColumn) 
        => Add(new TFileParameter { ParameterName = parameterName, DbType = dbType, SourceColumn = sourceColumn });

    /// <inheritdoc />
    public override void AddRange(Array values)
    {
        if (values is null)
            throw new ArgumentNullException(nameof(values));

        foreach (var parameter in values)
            Add(Cast(parameter));
    }

    /// <summary>
    /// Insert the specified parameter into the collection.
    /// </summary>
    /// <param name="index">Index of the existing parameter before which to insert the new one.</param>
    /// <param name="item">Parameter to insert.</param>
    public void Insert(int index, TFileParameter item) => InternalList.Insert(index, item);

    /// <inheritdoc />
    public override void Insert(int index, object value)
        => InternalList.Insert(index, Cast(value));

    /// <summary>
    /// Removes all items from the collection.
    /// </summary>
    public override void Clear() => InternalList.Clear();

    /// <inheritdoc />
    public override void CopyTo(Array array, int index)
        => ((ICollection)InternalList).CopyTo(array, index);

    /// <summary>
    /// Convert collection to a System.Array.
    /// </summary>
    /// <param name="array">Destination array.</param>
    /// <param name="arrayIndex">Starting index in destination array.</param>
    public void CopyTo(TFileParameter[] array, int arrayIndex)
        => InternalList.CopyTo(array, arrayIndex);

    /// <inheritdoc />
    protected override TFileParameter GetParameter(string parameterName)
        => InternalList.First(p => p.ParameterName == parameterName);

    /// <inheritdoc />
    protected override TFileParameter GetParameter(int index)
        => InternalList[index];


    protected void SetParameter(string parameterName, TFileParameter value)
    {
        var parameter = InternalList.First(p => p.ParameterName == parameterName);
        parameter.Value = value.Value;
    }

    protected override void SetParameter(string parameterName, DbParameter value)
        => SetParameter(parameterName, Cast(value));


    protected void SetParameter(int index, TFileParameter value)
    {
        var parameter = InternalList[index];
        parameter.Value = value.Value;
    }

    /// <inheritdoc />
    protected override void SetParameter(int index, DbParameter value)
        => SetParameter(index, Cast(value));


    /// <summary>
    /// Gets the <see cref="TFileParameter"/> at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the <see cref="TFileParameter"/> to retrieve.</param>
    /// <value>The <see cref="TFileParameter"/> at the specified index.</value>
    public new TFileParameter this[int index]
    {
        get { return GetParameter(index); }
        set { SetParameter(index, value); }
    }


    public TFileParameter this[string parameterName]
    {
        get { return GetParameter(parameterName); }
        set { SetParameter(parameterName, value); }
    }

    /// <inheritdoc />
    public override object SyncRoot => throw new NotImplementedException();

    private int CultureAwareCompare(string strA, string strB)
        => CultureInfo.CurrentCulture.CompareInfo.Compare(strA, strB, CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth | CompareOptions.IgnoreCase);

    private static TFileParameter Cast(object? value)
    {
        var castedValue = value as TFileParameter;
        if (castedValue is null)
            throw new InvalidCastException($"The value \"{value}\" is not of type \"{nameof(TFileParameter)}\" and cannot be used in this parameter collection.");

        return castedValue;
    }

}
