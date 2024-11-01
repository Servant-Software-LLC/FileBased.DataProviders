using EFCore.Common.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EFCore.Xml.Infrastructure.Internal;

/// <summary>
/// Provides information about the <see cref="XmlOptionsExtension"/>.
/// </summary>
public class XmlOptionsExtensionInfo : FileOptionsExtensionInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="XmlOptionsExtensionInfo"/> class.
    /// </summary>
    /// <param name="extension">The <see cref="XmlOptionsExtension"/> instance.</param>
    public XmlOptionsExtensionInfo(XmlOptionsExtension extension)
        : base(extension)
    {
    }

    protected override string ProviderName => "Xml";

    public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other) => other is XmlOptionsExtensionInfo;
}

