using EFCore.Common.Tests.Models;
using Microsoft.EntityFrameworkCore;

namespace EFCore.Json.Tests.Models;

public class BloggingContext : BloggingContextBase
{
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        if (string.IsNullOrEmpty(ConnectionString))
            throw new ArgumentNullException(nameof(ConnectionString));

        var builder = options.UseJson(ConnectionString);
        if (DataSourceProvider != null)
            builder = builder.UseDataSource(DataSourceProvider);

        builder.EnableSensitiveDataLogging();
    }
}
