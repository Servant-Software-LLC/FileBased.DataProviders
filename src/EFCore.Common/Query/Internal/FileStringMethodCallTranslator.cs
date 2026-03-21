#nullable enable
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace EFCore.Common.Query.Internal;

/// <summary>
/// Translates string method calls (Contains, StartsWith, EndsWith, ToUpper, ToLower)
/// and EF.Functions.Like to SQL equivalents.
/// </summary>
public class FileStringMethodCallTranslator : IMethodCallTranslator
{
    private static readonly MethodInfo _containsMethod =
        typeof(string).GetRuntimeMethod(nameof(string.Contains), new[] { typeof(string) })!;

    private static readonly MethodInfo _startsWithMethod =
        typeof(string).GetRuntimeMethod(nameof(string.StartsWith), new[] { typeof(string) })!;

    private static readonly MethodInfo _endsWithMethod =
        typeof(string).GetRuntimeMethod(nameof(string.EndsWith), new[] { typeof(string) })!;

    private static readonly MethodInfo _toUpperMethod =
        typeof(string).GetRuntimeMethod(nameof(string.ToUpper), Type.EmptyTypes)!;

    private static readonly MethodInfo _toLowerMethod =
        typeof(string).GetRuntimeMethod(nameof(string.ToLower), Type.EmptyTypes)!;

    private static readonly MethodInfo _efLikeMethod =
        typeof(DbFunctionsExtensions).GetRuntimeMethod(
            nameof(DbFunctionsExtensions.Like),
            new[] { typeof(DbFunctions), typeof(string), typeof(string) })!;

    private static readonly MethodInfo _efLikeWithEscapeMethod =
        typeof(DbFunctionsExtensions).GetRuntimeMethod(
            nameof(DbFunctionsExtensions.Like),
            new[] { typeof(DbFunctions), typeof(string), typeof(string), typeof(string) })!;

    private readonly ISqlExpressionFactory _sqlExpressionFactory;

    public FileStringMethodCallTranslator(ISqlExpressionFactory sqlExpressionFactory)
    {
        _sqlExpressionFactory = sqlExpressionFactory;
    }

    public SqlExpression? Translate(
        SqlExpression? instance,
        MethodInfo method,
        IReadOnlyList<SqlExpression> arguments,
        IDiagnosticsLogger<DbLoggerCategory.Query> logger)
    {
        if (method == _containsMethod && instance != null)
        {
            // string.Contains("x") → instance LIKE '%x%'
            return CreateLikeExpression(instance, arguments[0], "%", "%");
        }

        if (method == _startsWithMethod && instance != null)
        {
            // string.StartsWith("x") → instance LIKE 'x%'
            return CreateLikeExpression(instance, arguments[0], "", "%");
        }

        if (method == _endsWithMethod && instance != null)
        {
            // string.EndsWith("x") → instance LIKE '%x'
            return CreateLikeExpression(instance, arguments[0], "%", "");
        }

        if (method == _toUpperMethod && instance != null)
        {
            return _sqlExpressionFactory.Function(
                "UPPER",
                new[] { instance },
                nullable: true,
                argumentsPropagateNullability: new[] { true },
                typeof(string));
        }

        if (method == _toLowerMethod && instance != null)
        {
            return _sqlExpressionFactory.Function(
                "LOWER",
                new[] { instance },
                nullable: true,
                argumentsPropagateNullability: new[] { true },
                typeof(string));
        }

        if (method == _efLikeMethod)
        {
            // EF.Functions.Like(instance, pattern) → instance LIKE pattern
            return _sqlExpressionFactory.Like(arguments[1], arguments[2]);
        }

        if (method == _efLikeWithEscapeMethod)
        {
            // EF.Functions.Like(instance, pattern, escape) → instance LIKE pattern ESCAPE escape
            return _sqlExpressionFactory.Like(arguments[1], arguments[2], arguments[3]);
        }

        return null;
    }

    private SqlExpression CreateLikeExpression(
        SqlExpression instance,
        SqlExpression pattern,
        string prefix,
        string suffix)
    {
        // When the pattern is a constant, we can build the full LIKE pattern as a single constant.
        // This avoids generating SQL with || concatenation which the file-based SQL parser doesn't support.
        if (pattern is SqlConstantExpression constantPattern && constantPattern.Value is string patternValue)
        {
            var likePattern = $"{prefix}{EscapeLikePattern(patternValue)}{suffix}";
            return _sqlExpressionFactory.Like(
                instance,
                _sqlExpressionFactory.Constant(likePattern));
        }

        // For non-constant patterns, generate LIKE with concatenation (best-effort).
        var concatArgs = new List<SqlExpression>();
        if (prefix.Length > 0)
            concatArgs.Add(_sqlExpressionFactory.Constant(prefix));
        concatArgs.Add(pattern);
        if (suffix.Length > 0)
            concatArgs.Add(_sqlExpressionFactory.Constant(suffix));

        SqlExpression likeArg = concatArgs[0];
        for (int i = 1; i < concatArgs.Count; i++)
        {
            likeArg = _sqlExpressionFactory.Add(likeArg, concatArgs[i]);
        }

        return _sqlExpressionFactory.Like(instance, likeArg);
    }

    private static string EscapeLikePattern(string pattern)
    {
        // Escape LIKE special characters in the search value
        return pattern
            .Replace("%", "\\%")
            .Replace("_", "\\_");
    }
}
