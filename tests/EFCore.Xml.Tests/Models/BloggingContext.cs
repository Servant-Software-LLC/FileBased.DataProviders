using EFCore.Common.Tests.Models;
using Microsoft.EntityFrameworkCore;

namespace EFCore.Xml.Tests.Models;

public class BloggingContext : BloggingContextBase
{
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        if (string.IsNullOrEmpty(ConnectionString))
            throw new ArgumentNullException(nameof(ConnectionString));

        options.UseXml(ConnectionString).EnableSensitiveDataLogging();
    }
}
