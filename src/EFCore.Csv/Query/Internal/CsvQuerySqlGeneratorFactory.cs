using Microsoft.EntityFrameworkCore.Query;

namespace EFCore.Csv.Query.Internal;

public class CsvQuerySqlGeneratorFactory : IQuerySqlGeneratorFactory
{
    public CsvQuerySqlGeneratorFactory(QuerySqlGeneratorDependencies dependencies)
    {
        Dependencies = dependencies;
    }

    /// <summary>
    ///     Relational provider-specific dependencies for this service.
    /// </summary>
    protected virtual QuerySqlGeneratorDependencies Dependencies { get; }

    public virtual QuerySqlGenerator Create()
        => new CsvQuerySqlGenerator(Dependencies);
}
