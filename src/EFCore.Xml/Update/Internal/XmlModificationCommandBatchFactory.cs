using Microsoft.EntityFrameworkCore.Update;

namespace EFCore.Xml.Update.Internal;

internal class XmlModificationCommandBatchFactory : IModificationCommandBatchFactory
{
    public XmlModificationCommandBatchFactory(ModificationCommandBatchFactoryDependencies dependencies)
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
