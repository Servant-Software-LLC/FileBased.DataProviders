using Data.Common.DataSource;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace EFCore.Csv.Infrastructure.Internal;

public abstract class FileOptionsExtension : RelationalOptionsExtension
{
    public IDataSourceProvider DataSourceProvider { get; private set; }

    public FileOptionsExtension(IDataSourceProvider provider)
    {
        DataSourceProvider = provider;
    }

    protected FileOptionsExtension(FileOptionsExtension copyFrom)
        : base(copyFrom)
    {
        DataSourceProvider = copyFrom.DataSourceProvider;
    }

    public FileOptionsExtension WithDataSource(IDataSourceProvider provider)
    {
        if (provider == DataSourceProvider)
            return this;
    
        var clone = (FileOptionsExtension)Clone();
        clone.DataSourceProvider = provider;
        return clone;
    }


    public override void ApplyServices(IServiceCollection services)
    {
        if (DataSourceProvider != null)
            services.AddSingleton(DataSourceProvider);
    }

    /// <summary>
    ///     Finds an existing <see cref="FileOptionsExtension" /> registered on the given options
    ///     or throws if none has been registered. 
    /// </summary>
    /// <param name="options">The context options to look in.</param>
    /// <returns>The extension.</returns>
    public static FileOptionsExtension ExtractFileOptions(IDbContextOptions options)
    {
        if (options == null)
            throw new ArgumentNullException(nameof(options));

        var fileOptionsExtensions
            = options.Extensions
                .Where(x => x.GetType().IsAssignableTo(typeof(FileOptionsExtension)))
                .Cast<FileOptionsExtension>()
                .ToList();

        if (fileOptionsExtensions.Count == 0)
        {
            throw new InvalidOperationException("No file based database providers are configured.");
        }

        if (fileOptionsExtensions.Count > 1)
        {
            throw new InvalidOperationException("Multiple file based database provider configurations found. A context can only be configured to use a single database provider.");
        }

        return fileOptionsExtensions[0];
    }

}
