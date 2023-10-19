using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EFCore.Csv.Infrastructure.Internal;

public class CsvOptionsExtensionInfo : DbContextOptionsExtensionInfo
{
    public CsvOptionsExtensionInfo(CsvOptionsExtension extension)
        : base(extension)
    {
    }

    public override bool IsDatabaseProvider => true;

    public override string LogFragment => $"Using Csv Provider - ConnectionString: {ConnectionString}";

    public override int GetServiceProviderHashCode() => ConnectionString.GetHashCode();

    public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other) => other is CsvOptionsExtensionInfo;

    public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
    {
        debugInfo["Csv:ConnectionString"] = ConnectionString;
    }

    public override CsvOptionsExtension Extension => (CsvOptionsExtension)base.Extension;
    private string ConnectionString => Extension.Connection == null ?
                                            Extension.ConnectionString :
                                            Extension.Connection.ConnectionString;
}
