using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EFCore.Common.Scaffolding.Internal;

public class FileScaffoldingModelFactory : RelationalScaffoldingModelFactory
{
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
