using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Json;

/*
 * Because IDataParameterCollection is primarily an IList,
 * the sample can use an existing class for most of the implementation.
 */
public class JsonParameterCollection : ArrayList, IDataParameterCollection
{
    public object this[string index]
    {
        get => this[IndexOf(index)];
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

    public override int Add(object value) => Add((JsonParameter)value);

    public int Add(JsonParameter value)
    {
        if (((JsonParameter)value).ParameterName != null)
        {
            return base.Add(value);
        }
        else
            throw new ArgumentException("parameter must be named");
    }

    public int Add(string parameterName, DbType type) => Add(new JsonParameter(parameterName, type));

    public int Add(string parameterName, object value) => Add(new JsonParameter(parameterName, value));

    public int Add(string parameterName, DbType dbType, string sourceColumn) 
        => Add(new JsonParameter(parameterName, dbType, sourceColumn));

    private int _cultureAwareCompare(string strA, string strB)
        => CultureInfo.CurrentCulture.CompareInfo.Compare(strA, strB, CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth | CompareOptions.IgnoreCase);

}
