using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EFCore.Json.Infrastructure.Internal;

/// <summary>
/// Provides additional information regarding the JSON options extension.
/// </summary>
public class JsonOptionsExtensionInfo : DbContextOptionsExtensionInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JsonOptionsExtensionInfo"/> class.
    /// </summary>
    /// <param name="extension">The JSON options extension.</param>
    public JsonOptionsExtensionInfo(JsonOptionsExtension extension)
        : base(extension)
    {
    }

    /// <summary>
    /// Gets a value indicating whether this extension is a database provider.
    /// </summary>
    public override bool IsDatabaseProvider => true;

    /// <summary>
    /// Gets a log fragment to describe the JSON provider configuration.
    /// </summary>
    public override string LogFragment => $"Using Json Provider - ConnectionString: {ConnectionString}";

    /// <summary>
    /// Gets a hash code to differentiate between multiple configurations using this extension.
    /// </summary>
    public override int GetServiceProviderHashCode() => ConnectionString.GetHashCode();

    /// <summary>
    /// Determines if the given extension info should use the same service provider.
    /// </summary>
    /// <param name="other">The other extension info.</param>
    /// <returns>True if both are of type <see cref="JsonOptionsExtensionInfo"/>, false otherwise.</returns>
    public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other) => other is JsonOptionsExtensionInfo;

    /// <summary>
    /// Populates the debug information with details about the JSON connection.
    /// </summary>
    /// <param name="debugInfo">The dictionary to populate with debug information.</param>
    public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
    {
        debugInfo["Json:ConnectionString"] = ConnectionString;
    }

    /// <summary>
    /// Gets the underlying JSON options extension.
    /// </summary>
    public override JsonOptionsExtension Extension => (JsonOptionsExtension)base.Extension;

    /// <summary>
    /// Gets the connection string, either from the Extension's Connection property (if set) or directly from the Extension's ConnectionString property.
    /// </summary>
    private string ConnectionString => Extension.Connection == null ?
                                            Extension.ConnectionString :
                                            Extension.Connection.ConnectionString;
}
