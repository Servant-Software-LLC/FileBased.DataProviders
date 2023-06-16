using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace EFCore.Xml.Metadata.Conventions;

public class XmlConventionSetBuilder : RelationalConventionSetBuilder
{
    public XmlConventionSetBuilder(
    ProviderConventionSetBuilderDependencies dependencies,
    RelationalConventionSetBuilderDependencies relationalDependencies)
    : base(dependencies, relationalDependencies)
    {
    }

    public override ConventionSet CreateConventionSet() => base.CreateConventionSet();

}
