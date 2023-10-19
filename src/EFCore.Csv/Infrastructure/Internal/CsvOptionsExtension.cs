using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace EFCore.Csv.Infrastructure.Internal;

public class CsvOptionsExtension : RelationalOptionsExtension
{
    private CsvOptionsExtensionInfo _info;

    public CsvOptionsExtension() { }
    protected internal CsvOptionsExtension(CsvOptionsExtension copyFrom)
        : base(copyFrom)
    {

    }

    public override DbContextOptionsExtensionInfo Info => _info ??= new CsvOptionsExtensionInfo(this);

    public override void ApplyServices(IServiceCollection services)
    {
        services.AddEntityFrameworkCsv();
    }



    public override void Validate(IDbContextOptions options)
    {
        // You can add any validation logic here, if necessary.
    }

    protected override RelationalOptionsExtension Clone() => new CsvOptionsExtension(this);

}
