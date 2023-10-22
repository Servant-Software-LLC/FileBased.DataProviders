using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EFCore.Xml.Infrastructure.Internal;

public class XmlOptionsExtensionInfo : DbContextOptionsExtensionInfo
{
    public XmlOptionsExtensionInfo(XmlOptionsExtension extension)
        : base(extension)
    {
    }

    public override bool IsDatabaseProvider => true;

    public override string LogFragment => $"Using Xml Provider - ConnectionString: {ConnectionString}";

    public override int GetServiceProviderHashCode() => ConnectionString.GetHashCode();

    public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other) => other is XmlOptionsExtensionInfo;

    public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
    {
        debugInfo["Xml:ConnectionString"] = ConnectionString;
    }

    public override XmlOptionsExtension Extension => (XmlOptionsExtension)base.Extension;
    private string ConnectionString => Extension.Connection == null ?
                                            Extension.ConnectionString :
                                            Extension.Connection.ConnectionString;
}

