using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EFCore.Common.Scaffolding.Internal;


/// <summary>
/// Factory for creating a model based on a file.
/// </summary>
public class FileScaffoldingModelFactory : RelationalScaffoldingModelFactory
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FileScaffoldingModelFactory"/> class.
    /// </summary>
    /// <param name="operationReporter">The operation reporter.</param>
    /// <param name="candidateNamingService">The candidate naming service.</param>
    /// <param name="pluralizer">The pluralizer.</param>
    /// <param name="cSharpUtilities">The C# utilities.</param>
    /// <param name="scaffoldingTypeMapper">The scaffolding type mapper.</param>
    /// <param name="loggingDefinitions">The logging definitions.</param>
    /// <param name="modelRuntimeInitializer">The model runtime initializer.</param>
    public FileScaffoldingModelFactory(
        IOperationReporter operationReporter,
        ICandidateNamingService candidateNamingService,
        IPluralizer pluralizer,
        ICSharpUtilities cSharpUtilities,
        IScaffoldingTypeMapper scaffoldingTypeMapper,
        LoggingDefinitions loggingDefinitions,
        IModelRuntimeInitializer modelRuntimeInitializer)
        : base(operationReporter, candidateNamingService, pluralizer, cSharpUtilities, scaffoldingTypeMapper, loggingDefinitions, modelRuntimeInitializer)
    {
    }

}
