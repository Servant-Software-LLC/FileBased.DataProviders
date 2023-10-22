using Microsoft.EntityFrameworkCore.Query;

namespace EFCore.Json.Query.Internal;

public class JsonQuerySqlGeneratorFactory : IQuerySqlGeneratorFactory
{
    public JsonQuerySqlGeneratorFactory(QuerySqlGeneratorDependencies dependencies)
    {
        Dependencies = dependencies;
    }

    /// <summary>
    ///     Relational provider-specific dependencies for this service.
    /// </summary>
    protected virtual QuerySqlGeneratorDependencies Dependencies { get; }

    public virtual QuerySqlGenerator Create()
        => new JsonQuerySqlGenerator(Dependencies);
}
