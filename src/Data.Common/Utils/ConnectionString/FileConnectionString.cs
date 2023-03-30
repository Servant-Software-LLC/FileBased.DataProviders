using System.Diagnostics;
using System.Globalization;

namespace Data.Common.Utils.ConnectionString;

public class FileConnectionString : IConnectionStringProperties
{
    public FileConnectionString() { }
    public FileConnectionString(string connectionString) => ConnectionString = connectionString;

    public static implicit operator string(FileConnectionString connectionString) => connectionString.ConnectionString;
    public static implicit operator FileConnectionString(string connectionString) => new FileConnectionString(connectionString);

    public string ConnectionString 
    {
        get =>
            Formatted == null ?
                $"{nameof(FileConnectionStringKeywords.DataSource)}={DataSource};" :
                $"{nameof(FileConnectionStringKeywords.DataSource)}={DataSource}; {nameof(FileConnectionStringKeywords.Formatted)}={Formatted.Value};";

        set => Parse(value); 
    }

    public string DataSource { get; set; }
    public bool? Formatted { get; set; }

    public FileConnectionString Clone() =>
        new()
        {
            DataSource = DataSource,
            Formatted = Formatted
        };

    public FileConnectionString AddFormatted(bool? formatted) => new(this) { Formatted = formatted };

    private void Parse(string connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
            throw new ArgumentNullException(nameof(connectionString));

        DbConnectionStringBuilder connectionStringBuilder = new();
        connectionStringBuilder.ConnectionString = connectionString;

        //DataSource
        if (TryGetValue(connectionStringBuilder, FileConnectionStringKeywords.DataSource, out object? dataSource))
        {
            if (dataSource is not string sDataSource)
                throw new ArgumentException($"Invalid connection string: {nameof(FileConnectionStringKeywords.DataSource)} was not a string.", nameof(connectionString));

            DataSource = sDataSource;
        }
        else
        {
            throw new ArgumentException($"Invalid connection string: {nameof(FileConnectionStringKeywords.DataSource)} is a required connection string name/value pair.", nameof(connectionString));
        }

        foreach (KeyValuePair<string, object> keyValuePair in connectionStringBuilder)
        {
            //Any required keywords (like DataSource) have already been addressed above.
            if (IsKeyword(keyValuePair.Key, FileConnectionStringKeywords.DataSource))
                continue;


            //Formatted
            if (IsKeyword(keyValuePair.Key, FileConnectionStringKeywords.Formatted))
            {
                if (keyValuePair.Value == null)
                    throw new ArgumentException($"Invalid connection string: {nameof(FileConnectionStringKeywords.Formatted)} was null.", nameof(connectionString));

                var bFormatted = ConvertToBoolean(keyValuePair.Value);
                if (!bFormatted.HasValue)
                    throw new ArgumentException($"Invalid connection string: {nameof(FileConnectionStringKeywords.Formatted)} was not a boolean value.", nameof(connectionString));

                Formatted = bFormatted.Value;
                continue;
            }

            throw new ArgumentException($"Invalid connection string: Unknown keyword '{keyValuePair.Key}'", nameof(connectionString));
        }
    }

    private static bool? ConvertToBoolean(object value)
    {
        Debug.Assert(null != value, "ConvertToBoolean(null)");
        string? svalue = value as string;
        if (null != svalue)
        {
            if (StringComparer.OrdinalIgnoreCase.Equals(svalue, "true") || StringComparer.OrdinalIgnoreCase.Equals(svalue, "yes"))
                return true;
            else if (StringComparer.OrdinalIgnoreCase.Equals(svalue, "false") || StringComparer.OrdinalIgnoreCase.Equals(svalue, "no"))
                return false;
            else
            {
                string tmp = svalue.Trim();  // Remove leading & trailing white space.
                if (StringComparer.OrdinalIgnoreCase.Equals(tmp, "true") || StringComparer.OrdinalIgnoreCase.Equals(tmp, "yes"))
                    return true;
                else if (StringComparer.OrdinalIgnoreCase.Equals(tmp, "false") || StringComparer.OrdinalIgnoreCase.Equals(tmp, "no"))
                    return false;
            }
            return bool.Parse(svalue);
        }
        try
        {
            return ((IConvertible)value).ToBoolean(CultureInfo.InvariantCulture);
        }
        catch (InvalidCastException)
        {
            return null;
        }
    }

    private bool TryGetValue(DbConnectionStringBuilder connectionStringBuilder, FileConnectionStringKeywords connectionStringKeyword, out object? value)
    {
        if (connectionStringBuilder.TryGetValue(connectionStringKeyword.ToString(), out value))
            return true;

        var aliases = GetAliases(connectionStringKeyword);
        foreach(var alias in aliases)
        {
            if (connectionStringBuilder.TryGetValue(alias, out value))
                return true;
        }

        return false;
    }

    private static bool IsKeyword(string keyword, FileConnectionStringKeywords connectionStringKeyword) 
    {
        if (string.Compare(keyword, connectionStringKeyword.ToString(), true) == 0)
            return true;

        foreach (var aliasKeyword in GetAliases(connectionStringKeyword))
        {
            if (string.Compare(keyword, aliasKeyword, true) == 0)
                return true;
        }

        return false;
    }

    private static IEnumerable<string> GetAliases(FileConnectionStringKeywords keyword)
    {
        var enumType = typeof(FileConnectionStringKeywords);
        var memberInfos = enumType.GetMember(keyword.ToString());

        var enumValueMemberInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == enumType);
        if (enumValueMemberInfo == null)
            yield break;

        var valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(AliasAttribute), false);
        foreach(AliasAttribute valueAttribute in valueAttributes ) 
        {
            yield return valueAttribute.Name;
        }
    }
}
