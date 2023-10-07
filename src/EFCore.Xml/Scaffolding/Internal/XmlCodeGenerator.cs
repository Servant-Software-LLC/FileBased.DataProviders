using EFCore.Common.Utils;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding;
using System.Reflection;

namespace EFCore.Xml.Scaffolding.Internal;

public class XmlCodeGenerator : ProviderCodeGenerator
{
    private static readonly MethodInfo _useXmlMethodInfo
        = typeof(XmlDbContextOptionsExtensions).GetRequiredRuntimeMethod(
            nameof(XmlDbContextOptionsExtensions.UseXml),
            typeof(DbContextOptionsBuilder),
            typeof(string));

    public XmlCodeGenerator(ProviderCodeGeneratorDependencies dependencies)
            : base(dependencies)
    {

    }

    public override MethodCallCodeFragment GenerateUseProvider(
        string connectionString,
        MethodCallCodeFragment? providerOptions)
        => new(
            _useXmlMethodInfo,
            providerOptions == null
                ? new object[] { connectionString }
                : new object[] { connectionString, new NestedClosureCodeFragment("x", providerOptions) });

}
