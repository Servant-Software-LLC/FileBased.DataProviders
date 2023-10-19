using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace EFCore.Xml.Infrastructure.Internal;

public class XmlOptionsExtension : RelationalOptionsExtension
{
    private XmlOptionsExtensionInfo _info;

    public XmlOptionsExtension() { }
    protected internal XmlOptionsExtension(XmlOptionsExtension copyFrom)
        : base(copyFrom)
    {

    }

    public override DbContextOptionsExtensionInfo Info => _info ??= new XmlOptionsExtensionInfo(this);

    public override void ApplyServices(IServiceCollection services)
    {
        services.AddEntityFrameworkXml();
    }



    public override void Validate(IDbContextOptions options)
    {
        // You can add any validation logic here, if necessary.
    }

    protected override RelationalOptionsExtension Clone() => new XmlOptionsExtension(this);

}
