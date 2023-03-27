using EFCore.Xml.Infrastructure.Internal;

namespace Microsoft.EntityFrameworkCore.Infrastructure;

public class XmlDbContextOptionsBuilder : RelationalDbContextOptionsBuilder<XmlDbContextOptionsBuilder, XmlOptionsExtension>
{
    public XmlDbContextOptionsBuilder(DbContextOptionsBuilder optionsBuilder)
        : base(optionsBuilder)
    {
    }

}
