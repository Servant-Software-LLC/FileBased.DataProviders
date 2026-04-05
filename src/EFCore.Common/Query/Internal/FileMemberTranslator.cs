#nullable enable
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace EFCore.Common.Query.Internal;

/// <summary>
/// Translates member access expressions (string.Length, DateTime.Now, DateTime.UtcNow) to SQL equivalents.
/// </summary>
public class FileMemberTranslator : IMemberTranslator
{
    private readonly ISqlExpressionFactory _sqlExpressionFactory;

    public FileMemberTranslator(ISqlExpressionFactory sqlExpressionFactory)
    {
        _sqlExpressionFactory = sqlExpressionFactory;
    }

    public SqlExpression? Translate(
        SqlExpression? instance,
        MemberInfo member,
        Type returnType,
        IDiagnosticsLogger<DbLoggerCategory.Query> logger)
    {
        // string.Length → LENGTH(instance)
        if (member.DeclaringType == typeof(string) && member.Name == nameof(string.Length) && instance != null)
        {
            return _sqlExpressionFactory.Function(
                "LENGTH",
                new[] { instance },
                nullable: true,
                argumentsPropagateNullability: new[] { true },
                typeof(int));
        }

        // DateTime.Now → DATETIME('now', 'localtime')
        if (member.DeclaringType == typeof(DateTime) && member.Name == nameof(DateTime.Now))
        {
            return _sqlExpressionFactory.Function(
                "DATETIME",
                new SqlExpression[]
                {
                    _sqlExpressionFactory.Constant("now"),
                    _sqlExpressionFactory.Constant("localtime"),
                },
                nullable: false,
                argumentsPropagateNullability: new[] { false, false },
                typeof(DateTime));
        }

        // DateTime.UtcNow → DATETIME('now')
        if (member.DeclaringType == typeof(DateTime) && member.Name == nameof(DateTime.UtcNow))
        {
            return _sqlExpressionFactory.Function(
                "DATETIME",
                new SqlExpression[] { _sqlExpressionFactory.Constant("now") },
                nullable: false,
                argumentsPropagateNullability: new[] { false },
                typeof(DateTime));
        }

        return null;
    }
}
