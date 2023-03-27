using EFCore.JSON.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Microsoft.EntityFrameworkCore;

public static class JsonDbContextOptionsBuilderExtensions
{
    public static DbContextOptionsBuilder UseJson(
        this DbContextOptionsBuilder optionsBuilder,
        string connectionString,
        Action<JsonDbContextOptionsBuilder>? jsonOptionsAction = null)
    {
        if (optionsBuilder is null)
            throw new ArgumentNullException(nameof(optionsBuilder));

        var extension = (JsonOptionsExtension)GetOrCreateExtension(optionsBuilder).WithConnectionString(connectionString);
        ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(GetOrCreateExtension(optionsBuilder));
        ConfigureWarnings(optionsBuilder);

        jsonOptionsAction?.Invoke(new JsonDbContextOptionsBuilder(optionsBuilder));

        return optionsBuilder;
    }

    private static JsonOptionsExtension GetOrCreateExtension(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.Options.FindExtension<JsonOptionsExtension>()
               ?? (JsonOptionsExtension)new JsonOptionsExtension().WithConnectionString("Filename=:memory:");

    private static void ConfigureWarnings(DbContextOptionsBuilder optionsBuilder)
    {
        var coreOptionsExtension = optionsBuilder.Options.FindExtension<CoreOptionsExtension>()
            ?? new CoreOptionsExtension();

        coreOptionsExtension = RelationalOptionsExtension.WithDefaultWarningConfiguration(coreOptionsExtension);

        ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(coreOptionsExtension);
    }
}
