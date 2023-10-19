using EFCore.Common.Utils;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding;
using System.Reflection;

namespace EFCore.Xml.Scaffolding.Internal;

/// <summary>
/// Represents a code generator for the XML data source in Entity Framework Core.
/// This class facilitates the generation of provider-specific code fragments for the XML provider.
/// </summary>
public class XmlCodeGenerator : ProviderCodeGenerator
{
    // Holds the method info of the UseXml extension method for DbContext options builder.
    private static readonly MethodInfo _useXmlMethodInfo
        = typeof(XmlDbContextOptionsExtensions).GetRequiredRuntimeMethod(
            nameof(XmlDbContextOptionsExtensions.UseXml),
            typeof(DbContextOptionsBuilder),
            typeof(string));

    /// <summary>
    /// Initializes a new instance of the <see cref="XmlCodeGenerator"/> class.
    /// </summary>
    /// <param name="dependencies">The dependencies required for the code generator.</param>
    public XmlCodeGenerator(ProviderCodeGeneratorDependencies dependencies)
            : base(dependencies)
    {

    }

    /// <summary>
    /// Generates a method call code fragment for configuring the use of the XML provider.
    /// </summary>
    /// <param name="connectionString">The connection string for the XML data source.</param>
    /// <param name="providerOptions">Optional additional provider-specific configuration.</param>
    /// <returns>The method call code fragment for configuring the XML provider.</returns>
    public override MethodCallCodeFragment GenerateUseProvider(
        string connectionString,
        MethodCallCodeFragment? providerOptions)
        => new(
            _useXmlMethodInfo,
            providerOptions == null
                ? new object[] { connectionString }
                : new object[] { connectionString, new NestedClosureCodeFragment("x", providerOptions) });

}
