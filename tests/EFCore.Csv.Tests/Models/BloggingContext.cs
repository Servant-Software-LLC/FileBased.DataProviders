using EFCore.Common.Tests.Models;
using Microsoft.EntityFrameworkCore;

namespace EFCore.Csv.Tests.Models;

public class BloggingContext : BloggingContextBase
{
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        if (string.IsNullOrEmpty(ConnectionString))
            throw new ArgumentNullException(nameof(ConnectionString));

        var builder = options.UseCsv(ConnectionString);
        if (DataSourceProvider != null)
            builder = builder.UseDataSource(DataSourceProvider);

        builder.EnableSensitiveDataLogging();
    }
}
