using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace EFCore.Json.Metadata.Conventions;

public class JsonConventionSetBuilder : RelationalConventionSetBuilder
{
    public JsonConventionSetBuilder(
    ProviderConventionSetBuilderDependencies dependencies,
    RelationalConventionSetBuilderDependencies relationalDependencies)
    : base(dependencies, relationalDependencies)
    {
    }

    public override ConventionSet CreateConventionSet() => base.CreateConventionSet();

}
