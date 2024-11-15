namespace Data.Common.Extension;

public static class FloatingPointDataTypeExtensions
{
    public static Type GetClrType(this FloatingPointDataType preferredfloatingPointDataType, Type columnType)
    {
        // Float is the lowest precision floating point type in DataFrame, we can't downgrade any other type to float
        if (preferredfloatingPointDataType == FloatingPointDataType.Float)
            return columnType;

        // Convert a float or double to their preferred floating point type
        if (columnType == typeof(float) || columnType == typeof(double))
            return preferredfloatingPointDataType.ToType();

        // Otherwise, return the column type as is
        return columnType;
    }

    public static Type ToType(this FloatingPointDataType floatingPointDataType) =>
        floatingPointDataType switch
        {
            FloatingPointDataType.Float => typeof(float),
            FloatingPointDataType.Double => typeof(double),
            FloatingPointDataType.Decimal => typeof(decimal),
            _ => throw new ArgumentOutOfRangeException(nameof(floatingPointDataType), floatingPointDataType, null)
        };
}
