using Microsoft.EntityFrameworkCore.Query;

namespace EFCore.Common.Query.Internal;

/// <summary>
/// Provides member translations for file-based EF Core providers.
/// Registers string.Length and DateTime member translators.
/// </summary>
public class FileMemberTranslatorProvider : RelationalMemberTranslatorProvider
{
    public FileMemberTranslatorProvider(RelationalMemberTranslatorProviderDependencies dependencies)
        : base(dependencies)
    {
        var sqlExpressionFactory = dependencies.SqlExpressionFactory;

        AddTranslators(new IMemberTranslator[]
        {
            new FileMemberTranslator(sqlExpressionFactory),
        });
    }
}
