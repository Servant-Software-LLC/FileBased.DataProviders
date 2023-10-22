using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EFCore.Csv.Infrastructure.Internal;

/// <summary>
/// Provides information about the CsvOptionsExtension.
/// </summary>
public class CsvOptionsExtensionInfo : DbContextOptionsExtensionInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CsvOptionsExtensionInfo"/> class.
    /// </summary>
    /// <param name="extension">The CsvOptionsExtension instance.</param>
    public CsvOptionsExtensionInfo(CsvOptionsExtension extension)
        : base(extension)
    {
    }

    /// <summary>
    /// Gets a value indicating whether the extension is a database provider.
    /// </summary>
    public override bool IsDatabaseProvider => true;

    /// <summary>
    /// Gets a fragment representing the Csv provider configuration for logging.
    /// </summary>
    public override string LogFragment => $"Using Csv Provider - ConnectionString: {ConnectionString}";

    /// <summary>
    /// Gets a hash code for the service provider configuration.
    /// </summary>
    /// <returns>A hash code for the service provider configuration.</returns>
    public override int GetServiceProviderHashCode() => ConnectionString.GetHashCode();

    /// <summary>
    /// Determines whether the same service provider should be used for the given extension info.
    /// </summary>
    /// <param name="other">The other extension info.</param>
    /// <returns><c>true</c> if the same service provider should be used; otherwise, <c>false</c>.</returns>
    public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other) => other is CsvOptionsExtensionInfo;

    /// <summary>
    /// Populates a dictionary with information for debugging.
    /// </summary>
    /// <param name="debugInfo">The dictionary to be populated.</param>
    public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
    {
        debugInfo["Csv:ConnectionString"] = ConnectionString;
    }

    /// <summary>
    /// Gets the CsvOptionsExtension instance.
    /// </summary>
    public override CsvOptionsExtension Extension => (CsvOptionsExtension)base.Extension;

    /// <summary>
    /// Gets the connection string for the Csv provider.
    /// </summary>
    private string ConnectionString => Extension.Connection == null ?
                                            Extension.ConnectionString :
                                            Extension.Connection.ConnectionString;
}
