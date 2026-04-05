using System.Data.Common;

namespace System.Data.FileClient;

/// <summary>
/// Provides a base class for file-based command builders that automatically generate
/// INSERT, UPDATE, and DELETE commands based on a SELECT command.
/// </summary>
/// <typeparam name="TFileParameter">The type of the file parameter.</typeparam>
public abstract class FileCommandBuilder<TFileParameter> : DbCommandBuilder
    where TFileParameter : FileParameter<TFileParameter>, new()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FileCommandBuilder{TFileParameter}"/> class.
    /// </summary>
    protected FileCommandBuilder()
    {
        QuotePrefix = "[";
        QuoteSuffix = "]";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileCommandBuilder{TFileParameter}"/> class
    /// with the specified data adapter.
    /// </summary>
    /// <param name="adapter">The data adapter to generate commands for.</param>
    protected FileCommandBuilder(FileDataAdapter<TFileParameter> adapter) : this()
    {
        DataAdapter = adapter;
    }

    /// <inheritdoc/>
    protected override void ApplyParameterInfo(DbParameter parameter, DataRow row, StatementType statementType, bool whereClause)
    {
        // File-based providers don't need special parameter info beyond what the base class provides.
    }

    /// <inheritdoc/>
    protected override string GetParameterName(int parameterOrdinal)
    {
        return $"@p{parameterOrdinal}";
    }

    /// <inheritdoc/>
    protected override string GetParameterName(string parameterName)
    {
        return $"@{parameterName}";
    }

    /// <inheritdoc/>
    protected override string GetParameterPlaceholder(int parameterOrdinal)
    {
        return $"@p{parameterOrdinal}";
    }

    /// <inheritdoc/>
    public override string QuoteIdentifier(string unquotedIdentifier)
    {
        return $"{QuotePrefix}{unquotedIdentifier}{QuoteSuffix}";
    }

    /// <inheritdoc/>
    public override string UnquoteIdentifier(string quotedIdentifier)
    {
        var result = quotedIdentifier;
        if (result.StartsWith(QuotePrefix))
            result = result.Substring(QuotePrefix.Length);
        if (result.EndsWith(QuoteSuffix))
            result = result.Substring(0, result.Length - QuoteSuffix.Length);
        return result;
    }
}
