using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace EFCore.Csv.Infrastructure.Internal;

public class CsvOptionsExtension : RelationalOptionsExtension
{
    private MyCustomOptionsExtensionInfo _info;

    public CsvOptionsExtension() { }
    protected internal CsvOptionsExtension(CsvOptionsExtension copyFrom)
        : base(copyFrom)
    {

    }

    public override DbContextOptionsExtensionInfo Info => _info ??= new MyCustomOptionsExtensionInfo(this);

    public override void ApplyServices(IServiceCollection services)
    {
        services.AddEntityFrameworkCsv();
    }



    public override void Validate(IDbContextOptions options)
    {
        // You can add any validation logic here, if necessary.
    }

    protected override RelationalOptionsExtension Clone() => new CsvOptionsExtension(this);


    public class MyCustomOptionsExtensionInfo : DbContextOptionsExtensionInfo
    {
        public MyCustomOptionsExtensionInfo(CsvOptionsExtension extension)
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

        public override CsvOptionsExtension Extension => (CsvOptionsExtension)base.Extension;
        private string ConnectionString => Extension.Connection == null ?
                                                Extension.ConnectionString :
                                                Extension.Connection.ConnectionString;
    }

}
