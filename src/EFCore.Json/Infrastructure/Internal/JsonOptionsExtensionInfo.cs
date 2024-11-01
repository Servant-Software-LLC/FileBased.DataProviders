using EFCore.Common.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EFCore.Json.Infrastructure.Internal;

/// <summary>
/// Provides information about the <see cref="JsonOptionsExtension"/>.
/// </summary>
public class JsonOptionsExtensionInfo : FileOptionsExtensionInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JsonOptionsExtensionInfo"/> class.
    /// </summary>
    /// <param name="extension">The <see cref="JsonOptionsExtension"/> instance.</param>
    public JsonOptionsExtensionInfo(JsonOptionsExtension extension)
        : base(extension)
    {
    }

    protected override string ProviderName => "Json";

    /// <summary>
    /// Determines if the given extension info should use the same service provider.
    /// </summary>
    /// <param name="other">The other extension info.</param>
    /// <returns>True if both are of type <see cref="JsonOptionsExtensionInfo"/>, false otherwise.</returns>
    public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other) => other is JsonOptionsExtensionInfo;
}
