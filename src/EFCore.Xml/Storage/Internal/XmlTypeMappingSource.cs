using Microsoft.EntityFrameworkCore.Storage;

namespace EFCore.Xml.Storage.Internal;

public class XmlTypeMappingSource : RelationalTypeMappingSource
{
    public XmlTypeMappingSource(
        TypeMappingSourceDependencies dependencies,
        RelationalTypeMappingSourceDependencies relationalDependencies)
        : base(dependencies, relationalDependencies)
    {
    }

    protected override RelationalTypeMapping FindMapping(in RelationalTypeMappingInfo mappingInfo)
    {
        var clrType = mappingInfo.ClrType;
        var storeTypeName = mappingInfo.StoreTypeName;

        if (storeTypeName != null)
        {
            switch (storeTypeName.ToLowerInvariant())
            {
                case "text":
                    return new StringTypeMapping("TEXT", System.Data.DbType.String);
            }
        }

        if (clrType != null)
        {
            if (clrType == typeof(int))
                return new IntTypeMapping("INTEGER", System.Data.DbType.Int32);
            if (clrType == typeof(double))
                return new DoubleTypeMapping("REAL", System.Data.DbType.Double);
            if (clrType == typeof(string))
                return new StringTypeMapping("TEXT", System.Data.DbType.String);
            if (clrType == typeof(bool))
                return new BoolTypeMapping("BOOLEAN", System.Data.DbType.Boolean);
            if (clrType == typeof(decimal))
                return new DecimalTypeMapping("DECIMAL", System.Data.DbType.Decimal);
            if (clrType == typeof(float))
                return new FloatTypeMapping("REAL", System.Data.DbType.Single);
            if (clrType == typeof(byte[]))
                return new ByteArrayTypeMapping("BLOB", System.Data.DbType.Binary);
            if (clrType == typeof(DateTime))
                return new DateTimeTypeMapping("DATETIME", System.Data.DbType.DateTime);
            if (clrType == typeof(DateTimeOffset))
                return new DateTimeOffsetTypeMapping("DATETIMEOFFSET", System.Data.DbType.DateTimeOffset);
            if (clrType == typeof(Guid))
                return new GuidTypeMapping("GUID", System.Data.DbType.Guid);
            if (clrType == typeof(long))
                return new LongTypeMapping("BIGINT", System.Data.DbType.Int64);
            if (clrType == typeof(short))
                return new ShortTypeMapping("SMALLINT", System.Data.DbType.Int16);
            if (clrType == typeof(byte))
                return new ByteTypeMapping("TINYINT", System.Data.DbType.Byte);
            if (clrType == typeof(DateOnly))
                return new DateOnlyTypeMapping("DATE");
            if (clrType == typeof(TimeOnly))
                return new TimeOnlyTypeMapping("TIME");
            if (clrType == typeof(TimeSpan))
                return new TimeSpanTypeMapping("TIME", System.Data.DbType.Time);
        }

        var baseMapping = base.FindMapping(mappingInfo);
        return baseMapping;
    }
}
