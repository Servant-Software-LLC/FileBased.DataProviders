using System.Globalization;

namespace System.Data.JsonClient;


/*
 * Because IDataParameterCollection is primarily an IList,
 * the sample can use an existing class for most of the implementation.
 */
public class JsonParameterCollection : ArrayList, IDataParameterCollection
{
    public object this[string index]
    {
        get => this[IndexOf(index)]!;
        set => this[IndexOf(index)] = value;
    }

    public bool Contains(string parameterName) => -1 != IndexOf(parameterName);

    public int IndexOf(string parameterName)
    {
        int index = 0;
        foreach (JsonParameter item in this)
        {
            if (0 == _cultureAwareCompare(item.ParameterName, parameterName))
            {
                return index;
            }
            index++;
        }
        return -1;
    }

    public void RemoveAt(string parameterName) => RemoveAt(IndexOf(parameterName));

    public override int Add(object? value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));

        if (value is not JsonParameter jsonParameter)
            throw new ArgumentException($"{value.GetType().Name} is not a {nameof(JsonParameter)}", nameof(value));

        return Add(jsonParameter);
    }

    public int Add(JsonParameter value)
    {
        if (string.IsNullOrEmpty(value.ParameterName))
            throw new ArgumentException($"{nameof(value.ParameterName)} property of the {nameof(JsonParameter)} must be named");

        return base.Add(value);
    }

    public int Add(string parameterName, DbType type) => Add(new JsonParameter(parameterName, type));

    public int Add(string parameterName, object value) => Add(new JsonParameter(parameterName, value));

    public int Add(string parameterName, DbType dbType, string sourceColumn)
        => Add(new JsonParameter(parameterName, dbType, sourceColumn));

    private int _cultureAwareCompare(string strA, string strB)
        => CultureInfo.CurrentCulture.CompareInfo.Compare(strA, strB, CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth | CompareOptions.IgnoreCase);

}
