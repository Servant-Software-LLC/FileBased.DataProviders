using Microsoft.EntityFrameworkCore.Query;

namespace EFCore.Xml.Query.Internal;

public class XmlQuerySqlGeneratorFactory : IQuerySqlGeneratorFactory
{
    public XmlQuerySqlGeneratorFactory(QuerySqlGeneratorDependencies dependencies)
    {
        Dependencies = dependencies;
    }

    /// <summary>
    ///     Relational provider-specific dependencies for this service.
    /// </summary>
    protected virtual QuerySqlGeneratorDependencies Dependencies { get; }

    public virtual QuerySqlGenerator Create()
        => new XmlQuerySqlGenerator(Dependencies);
}
