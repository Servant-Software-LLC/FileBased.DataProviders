using EFCore.Common.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EFCore.Csv.Infrastructure.Internal;

/// <summary>
/// Provides information about the <see cref="CsvOptionsExtension"/>.
/// </summary>
public class CsvOptionsExtensionInfo : FileOptionsExtensionInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CsvOptionsExtensionInfo"/> class.
    /// </summary>
    /// <param name="extension">The <see cref="CsvOptionsExtension"/> instance.</param>
    public CsvOptionsExtensionInfo(CsvOptionsExtension extension)
        : base(extension)
    {
    }

    protected override string ProviderName => "Csv";

    /// <summary>
    /// Determines whether the same service provider should be used for the given extension info.
    /// </summary>
    /// <param name="other">The other extension info.</param>
    /// <returns><c>true</c> if the same service provider should be used; otherwise, <c>false</c>.</returns>
    public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other) => 
        other is CsvOptionsExtensionInfo;
}
