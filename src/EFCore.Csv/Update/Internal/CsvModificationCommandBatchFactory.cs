using Microsoft.EntityFrameworkCore.Update;

namespace EFCore.Csv.Update.Internal;

internal class CsvModificationCommandBatchFactory : IModificationCommandBatchFactory
{
    public CsvModificationCommandBatchFactory(ModificationCommandBatchFactoryDependencies dependencies)
    {
        Dependencies = dependencies;
    }

    /// <summary>
    ///     Relational provider-specific dependencies for this service.
    /// </summary>
    protected virtual ModificationCommandBatchFactoryDependencies Dependencies { get; }

    //Note:  Create your own derived class, if needed for your custom provider
    public virtual ModificationCommandBatch Create()
        => new SingularModificationCommandBatch(Dependencies);
}
