using EFCore.Csv.Infrastructure.Internal;

namespace Microsoft.EntityFrameworkCore.Infrastructure;

public class CsvDbContextOptionsBuilder : RelationalDbContextOptionsBuilder<CsvDbContextOptionsBuilder, CsvOptionsExtension>
{
    public CsvDbContextOptionsBuilder(DbContextOptionsBuilder optionsBuilder)
        : base(optionsBuilder)
    {
    }

}
