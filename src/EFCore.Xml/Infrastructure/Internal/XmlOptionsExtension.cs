using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace EFCore.Xml.Infrastructure.Internal;

public class XmlOptionsExtension : RelationalOptionsExtension
{
    private MyCustomOptionsExtensionInfo? _info;

    public XmlOptionsExtension() { }
    protected internal XmlOptionsExtension(XmlOptionsExtension copyFrom)
        : base(copyFrom)
    {

    }

    public override DbContextOptionsExtensionInfo Info => _info ??= new MyCustomOptionsExtensionInfo(this);

    public override void ApplyServices(IServiceCollection services)
    {
        services.AddEntityFrameworkMyCustom();
    }



    public override void Validate(IDbContextOptions options)
    {
        // You can add any validation logic here, if necessary.
    }

    protected override RelationalOptionsExtension Clone() => new XmlOptionsExtension(this);


    public class MyCustomOptionsExtensionInfo : DbContextOptionsExtensionInfo
    {
        public MyCustomOptionsExtensionInfo(XmlOptionsExtension extension)
            : base(extension)
        {
        }

        public override bool IsDatabaseProvider => true;

        public override string LogFragment => $"Using Custom SQLite Provider - ConnectionString: {ConnectionString}";

        public override int GetServiceProviderHashCode() => ConnectionString.GetHashCode();

        public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other) => other is MyCustomOptionsExtensionInfo;

        public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
        {
            debugInfo["MyCustom:ConnectionString"] = ConnectionString;
        }

        public override XmlOptionsExtension Extension => (XmlOptionsExtension)base.Extension;
        private string? ConnectionString => Extension.Connection == null ?
                                                Extension.ConnectionString :
                                                Extension.Connection.ConnectionString;
    }

}
