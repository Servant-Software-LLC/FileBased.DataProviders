using Microsoft.Extensions.Logging;
using System.Data.Common;

namespace Data.Common.Utils.ConnectionString;

public class FileConnectionStringBuilder : DbConnectionStringBuilder
{
    public string DataSource
    {
        get => ContainsKey("DataSource") ? (string)this["DataSource"] : null;
        set => this["DataSource"] = value;
    }

    public LogLevel? LogLevel
    {
        get
        {
            if (!ContainsKey("LogLevel")) return null;
            if (Enum.TryParse(this["LogLevel"]?.ToString(), true, out Microsoft.Extensions.Logging.LogLevel level))
                return level;
            return null;
        }
        set
        {
            if (value == null) Remove("LogLevel");
            else this["LogLevel"] = value.ToString();
        }
    }

    public FloatingPointDataType? PreferredFloatingPointDataType
    {
        get
        {
            if (!ContainsKey("PreferredFloatingPointDataType")) return null;
            if (Enum.TryParse(this["PreferredFloatingPointDataType"]?.ToString(), true, out FloatingPointDataType fpt))
                return fpt;
            return null;
        }
        set
        {
            if (value == null) Remove("PreferredFloatingPointDataType");
            else this["PreferredFloatingPointDataType"] = value.ToString();
        }
    }

    public bool? CreateIfNotExist
    {
        get
        {
            if (!ContainsKey("CreateIfNotExist")) return null;
            if (bool.TryParse(this["CreateIfNotExist"]?.ToString(), out bool b)) return b;
            return null;
        }
        set
        {
            if (value == null) Remove("CreateIfNotExist");
            else this["CreateIfNotExist"] = value.ToString();
        }
    }
}
