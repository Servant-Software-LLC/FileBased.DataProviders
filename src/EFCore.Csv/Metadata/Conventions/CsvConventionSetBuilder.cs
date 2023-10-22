using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace EFCore.Csv.Metadata.Conventions;

public class CsvConventionSetBuilder : RelationalConventionSetBuilder
{
    public CsvConventionSetBuilder(
    ProviderConventionSetBuilderDependencies dependencies,
    RelationalConventionSetBuilderDependencies relationalDependencies)
    : base(dependencies, relationalDependencies)
    {
    }

    public override ConventionSet CreateConventionSet() => base.CreateConventionSet();

}
