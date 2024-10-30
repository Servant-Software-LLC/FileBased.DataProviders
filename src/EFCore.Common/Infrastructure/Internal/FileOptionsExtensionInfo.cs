using EFCore.Csv.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EFCore.Common.Infrastructure.Internal;

public abstract class FileOptionsExtensionInfo : DbContextOptionsExtensionInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FileOptionsExtensionInfo"/> class.
    /// </summary>
    /// <param name="extension">The <see cref="FileOptionsExtension"/> instance.</param>
    public FileOptionsExtensionInfo(FileOptionsExtension extension)
        : base(extension)
    {
    }

    protected abstract string ProviderName { get; }

    /// <summary>
    /// Gets a fragment representing the provider configuration for logging.
    /// </summary>
    public override string LogFragment => $"Using {ProviderName} Provider - ConnectionString: {ConnectionString}";

    /// <summary>
    /// Populates a dictionary with information for debugging.
    /// </summary>
    /// <param name="debugInfo">The dictionary to be populated.</param>
    public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
    {
        debugInfo[$"{ProviderName}:ConnectionString"] = ConnectionString;
    }

    /// <summary>
    /// Gets a value indicating whether the extension is a database provider.
    /// </summary>
    public override bool IsDatabaseProvider => true;

    /// <summary>
    /// Gets a hash code for the service provider configuration.
    /// </summary>
    /// <returns>A hash code for the service provider configuration.</returns>
    public override int GetServiceProviderHashCode()
    {
        if (Extension.DataSourceProvider == null)
            return ConnectionString.GetHashCode();

        return Extension.DataSourceProvider.GetHashCode();
    }

    /// <summary>
    /// Gets the FileOptionsExtension instance.
    /// </summary>
    public override FileOptionsExtension Extension => (FileOptionsExtension)base.Extension;

    /// <summary>
    /// Gets the connection string for the Csv provider.
    /// </summary>
    protected string ConnectionString => Extension.Connection == null ?
                                            Extension.ConnectionString :
                                            Extension.Connection.ConnectionString;
}
