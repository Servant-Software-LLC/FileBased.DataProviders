using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Microsoft.EntityFrameworkCore;

public static class CsvDbContextOptionsBuilderExtensions
{
    public static DbContextOptionsBuilder UseCsv(
        this DbContextOptionsBuilder optionsBuilder,
        string connectionString,
        Action<CsvDbContextOptionsBuilder>? csvOptionsAction = null)
    {
        //TODO: 


        return optionsBuilder;
    }
}
