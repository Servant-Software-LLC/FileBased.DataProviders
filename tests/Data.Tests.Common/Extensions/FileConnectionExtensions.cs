using Data.Common.Interfaces;
using Data.Common.Utils.ConnectionString;

namespace Data.Tests.Common.Extensions;

public static class FileConnectionExtensions
{
    public static object GetProperlyTypedValue(this IFileConnection fileConnection, float value)
    {
        if (fileConnection.DataTypeAlwaysString)
        {
            return value.ToString();
        }

        switch (fileConnection.PreferredFloatingPointDataType)
        {
            case FloatingPointDataType.Float:
                return value;
            case FloatingPointDataType.Double:
                return (double)value;
            case FloatingPointDataType.Decimal:
                return (decimal)value;
            default:
                return value;
        }
    }
}
