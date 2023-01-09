using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace EFCore.CSV.Infrastructure.Internal;

public class CsvOptionsExtension : RelationalOptionsExtension
{
    private DbContextOptionsExtensionInfo? _info;

    public CsvOptionsExtension()
    {
    }

    public CsvOptionsExtension(CsvOptionsExtension copyFrom) : base(copyFrom)
    {
        //TODO: Copy all option properties of copyFrom into the properties of this class.
    }

    //TODO: Provide readonly properties for each option needed for this provider

    //TODO: Provider a fluent style method to set option property.  Example: WithPostgresVersion() method at https://github.com/npgsql/efcore.pg/blob/main/src/EFCore.PG/Infrastructure/Internal/NpgsqlOptionsExtension.cs#L186

    public override DbContextOptionsExtensionInfo Info => _info ??= new ExtensionInfo(this);

    public override void ApplyServices(IServiceCollection services)
    {
        throw new NotImplementedException();
    }

    protected override RelationalOptionsExtension Clone() => new CsvOptionsExtension(this);

    private sealed class ExtensionInfo : RelationalExtensionInfo
    {
        private int? _serviceProviderHash;

        public ExtensionInfo(IDbContextOptionsExtension extension)
            : base(extension)
        {
        }

        public override int GetServiceProviderHashCode()
        {
            if (_serviceProviderHash == null)
            {
                var hashCode = new HashCode();
                hashCode.Add(base.GetServiceProviderHashCode());

                //TODO: Add to hashCode each option property.  Example: https://github.com/npgsql/efcore.pg/blob/main/src/EFCore.PG/Infrastructure/Internal/NpgsqlOptionsExtension.cs#L414
                //  hashCode.Add(Extension.PostgresVersion);

                _serviceProviderHash = hashCode.ToHashCode();
            }

            return _serviceProviderHash.Value;

        }

        public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
        {
            //TODO: Add to debugInfo each option property as a string of its GetHashCode() value.  Example: https://github.com/npgsql/efcore.pg/blob/main/src/EFCore.PG/Infrastructure/Internal/NpgsqlOptionsExtension.cs#L433
            //  debugInfo["ServantSoftware.EntityFrameworkCore.Csv:" + nameof(NpgsqlDbContextOptionsBuilder.SetPostgresVersion)]
            //    = (Extension.PostgresVersion?.GetHashCode() ?? 0).ToString(CultureInfo.InvariantCulture);
        }
    }

}
