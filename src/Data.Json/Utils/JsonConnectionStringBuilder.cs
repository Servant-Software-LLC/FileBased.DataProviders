using Data.Common.Utils.ConnectionString;

namespace Data.Json.Utils;

public class JsonConnectionStringBuilder : FileConnectionStringBuilder
{
    public bool? Formatted
    {
        get
        {
            if (!ContainsKey("Formatted")) return null;
            if (bool.TryParse(this["Formatted"]?.ToString(), out bool b)) return b;
            return null;
        }
        set
        {
            if (value == null) Remove("Formatted");
            else this["Formatted"] = value.ToString();
        }
    }
}
