using Microsoft.EntityFrameworkCore.Query;

namespace EFCore.Common.Query.Internal;

/// <summary>
/// Provides method call translations for file-based EF Core providers.
/// Registers string, math, and EF.Functions translators.
/// </summary>
public class FileMethodCallTranslatorProvider : RelationalMethodCallTranslatorProvider
{
    public FileMethodCallTranslatorProvider(RelationalMethodCallTranslatorProviderDependencies dependencies)
        : base(dependencies)
    {
        var sqlExpressionFactory = dependencies.SqlExpressionFactory;

        AddTranslators(new IMethodCallTranslator[]
        {
            new FileStringMethodCallTranslator(sqlExpressionFactory),
            new FileMathMethodCallTranslator(sqlExpressionFactory),
        });
    }
}
