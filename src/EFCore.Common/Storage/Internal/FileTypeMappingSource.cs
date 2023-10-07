using Microsoft.EntityFrameworkCore.Storage;

namespace EFCore.Common.Storage.Internal;

public class FileTypeMappingSource : RelationalTypeMappingSource
{
    // Dictionary to map the System.Type.FullName to the CLR type.
    private static readonly Dictionary<string, RelationalTypeMapping> _typeMappings = new Dictionary<string, RelationalTypeMapping>
    {
        { typeof(string).FullName, new StringTypeMapping("System.String", System.Data.DbType.String) },
        { typeof(int).FullName, new IntTypeMapping("System.Int32", System.Data.DbType.Int32) },
        { typeof(bool).FullName, new BoolTypeMapping("System.Boolean", System.Data.DbType.Boolean) },
        { typeof(decimal).FullName, new DecimalTypeMapping("System.Decimal", System.Data.DbType.Decimal) },

        // Add other mappings as needed.
    };

    public FileTypeMappingSource(TypeMappingSourceDependencies dependencies, RelationalTypeMappingSourceDependencies relationalDependencies)
        : base(dependencies, relationalDependencies)
    {
    }

    protected override RelationalTypeMapping FindMapping(in RelationalTypeMappingInfo mappingInfo)
    {
        var fullName = mappingInfo.ClrType != null ? mappingInfo.ClrType.FullName : mappingInfo.StoreTypeName;

        if (_typeMappings.TryGetValue(fullName, out var mapping))
        {
            return mapping;
        }

        // If you can't find the type, delegate to the base method, or you can return null.
        return base.FindMapping(mappingInfo);
    }
}
