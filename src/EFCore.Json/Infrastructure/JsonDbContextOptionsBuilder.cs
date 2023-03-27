using EFCore.JSON.Infrastructure.Internal;

namespace Microsoft.EntityFrameworkCore.Infrastructure;

public class JsonDbContextOptionsBuilder : RelationalDbContextOptionsBuilder<JsonDbContextOptionsBuilder, JsonOptionsExtension>
{
    public JsonDbContextOptionsBuilder(DbContextOptionsBuilder optionsBuilder)
        : base(optionsBuilder)
    {
    }

}
