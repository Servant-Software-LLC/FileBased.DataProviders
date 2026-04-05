#nullable enable
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace EFCore.Common.Query.Internal;

/// <summary>
/// Translates Math method calls (Abs, Ceiling, Floor, Round, Max, Min, etc.) to SQL equivalents.
/// </summary>
public class FileMathMethodCallTranslator : IMethodCallTranslator
{
    private static readonly Dictionary<MethodInfo, string> _supportedMethods = new()
    {
        { typeof(Math).GetRuntimeMethod(nameof(Math.Abs), new[] { typeof(double) })!, "ABS" },
        { typeof(Math).GetRuntimeMethod(nameof(Math.Abs), new[] { typeof(float) })!, "ABS" },
        { typeof(Math).GetRuntimeMethod(nameof(Math.Abs), new[] { typeof(int) })!, "ABS" },
        { typeof(Math).GetRuntimeMethod(nameof(Math.Abs), new[] { typeof(long) })!, "ABS" },
        { typeof(Math).GetRuntimeMethod(nameof(Math.Abs), new[] { typeof(decimal) })!, "ABS" },
        { typeof(Math).GetRuntimeMethod(nameof(Math.Abs), new[] { typeof(short) })!, "ABS" },
        { typeof(Math).GetRuntimeMethod(nameof(Math.Max), new[] { typeof(double), typeof(double) })!, "MAX" },
        { typeof(Math).GetRuntimeMethod(nameof(Math.Max), new[] { typeof(float), typeof(float) })!, "MAX" },
        { typeof(Math).GetRuntimeMethod(nameof(Math.Max), new[] { typeof(int), typeof(int) })!, "MAX" },
        { typeof(Math).GetRuntimeMethod(nameof(Math.Max), new[] { typeof(long), typeof(long) })!, "MAX" },
        { typeof(Math).GetRuntimeMethod(nameof(Math.Max), new[] { typeof(decimal), typeof(decimal) })!, "MAX" },
        { typeof(Math).GetRuntimeMethod(nameof(Math.Min), new[] { typeof(double), typeof(double) })!, "MIN" },
        { typeof(Math).GetRuntimeMethod(nameof(Math.Min), new[] { typeof(float), typeof(float) })!, "MIN" },
        { typeof(Math).GetRuntimeMethod(nameof(Math.Min), new[] { typeof(int), typeof(int) })!, "MIN" },
        { typeof(Math).GetRuntimeMethod(nameof(Math.Min), new[] { typeof(long), typeof(long) })!, "MIN" },
        { typeof(Math).GetRuntimeMethod(nameof(Math.Min), new[] { typeof(decimal), typeof(decimal) })!, "MIN" },
        { typeof(Math).GetRuntimeMethod(nameof(Math.Round), new[] { typeof(double) })!, "ROUND" },
        { typeof(Math).GetRuntimeMethod(nameof(Math.Round), new[] { typeof(decimal) })!, "ROUND" },
        { typeof(Math).GetRuntimeMethod(nameof(Math.Round), new[] { typeof(double), typeof(int) })!, "ROUND" },
        { typeof(Math).GetRuntimeMethod(nameof(Math.Round), new[] { typeof(decimal), typeof(int) })!, "ROUND" },
        { typeof(MathF).GetRuntimeMethod(nameof(MathF.Abs), new[] { typeof(float) })!, "ABS" },
        { typeof(MathF).GetRuntimeMethod(nameof(MathF.Round), new[] { typeof(float) })!, "ROUND" },
    };

    private readonly ISqlExpressionFactory _sqlExpressionFactory;

    public FileMathMethodCallTranslator(ISqlExpressionFactory sqlExpressionFactory)
    {
        _sqlExpressionFactory = sqlExpressionFactory;
    }

    public SqlExpression? Translate(
        SqlExpression? instance,
        MethodInfo method,
        IReadOnlyList<SqlExpression> arguments,
        IDiagnosticsLogger<DbLoggerCategory.Query> logger)
    {
        if (_supportedMethods.TryGetValue(method, out var sqlFunctionName))
        {
            var nullability = arguments.Select(_ => true).ToArray();
            return _sqlExpressionFactory.Function(
                sqlFunctionName,
                arguments,
                nullable: true,
                argumentsPropagateNullability: nullability,
                method.ReturnType);
        }

        return null;
    }
}
