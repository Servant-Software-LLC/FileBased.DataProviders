using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Microsoft.EntityFrameworkCore;

public static class XmlDbContextOptionsBuilderExtensions
{
    public static DbContextOptionsBuilder UseXml(
        this DbContextOptionsBuilder optionsBuilder,
        string connectionString,
        Action<XmlDbContextOptionsBuilder>? xmlOptionsAction = null)
    {
        //TODO: 


        return optionsBuilder;
    }

}
