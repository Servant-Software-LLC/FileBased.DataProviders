#nullable enable

using Microsoft.EntityFrameworkCore.Update;
using System.Text;

namespace EFCore.Common.Update.Internal;

/// <summary>
/// Base class for file-based EF Core providers' SQL generators.
/// Overrides EF Core 10's RETURNING-based approach with the legacy pattern
/// of separate SELECT statements that the file-based SQL parser supports.
/// </summary>
public abstract class FileUpdateSqlGeneratorBase : UpdateSqlGenerator
{
    protected FileUpdateSqlGeneratorBase(UpdateSqlGeneratorDependencies dependencies)
        : base(dependencies)
    {
    }

    public override ResultSetMapping AppendInsertReturningOperation(
        StringBuilder commandStringBuilder,
        IReadOnlyModificationCommand command,
        int commandPosition,
        out bool requiresTransaction)
    {
        var name = command.TableName;
        var schema = command.Schema;
        var writeOperations = command.ColumnModifications.Where(o => o.IsWrite).ToList();
        var readOperations = command.ColumnModifications.Where(o => o.IsRead).ToList();

        // Generate plain INSERT
        AppendInsertCommand(commandStringBuilder, name, schema!, writeOperations, readOperations);

        if (readOperations.Count > 0)
        {
            // Generate SELECT to read back identity columns using LAST_INSERT_ID()
            var keyOperations = readOperations.Where(o => o.IsKey).ToList();
            if (keyOperations.Count > 0)
            {
                commandStringBuilder.AppendLine();
                commandStringBuilder.Append("SELECT ");
                for (int i = 0; i < readOperations.Count; i++)
                {
                    if (i > 0) commandStringBuilder.Append(", ");
                    commandStringBuilder.Append(SqlGenerationHelper.DelimitIdentifier(readOperations[i].ColumnName));
                }
                commandStringBuilder
                    .Append(" FROM ")
                    .Append(SqlGenerationHelper.DelimitIdentifier(name, schema));

                commandStringBuilder.Append(" WHERE ");
                for (int i = 0; i < keyOperations.Count; i++)
                {
                    if (i > 0) commandStringBuilder.Append(" AND ");
                    commandStringBuilder.AppendFormat(
                        "{0}=LAST_INSERT_ID()",
                        SqlGenerationHelper.DelimitIdentifier(keyOperations[i].ColumnName));
                }
            }
        }

        requiresTransaction = false;
        return readOperations.Count > 0 ? ResultSetMapping.LastInResultSet : ResultSetMapping.NoResults;
    }

    public override ResultSetMapping AppendUpdateOperation(
        StringBuilder commandStringBuilder,
        IReadOnlyModificationCommand command,
        int commandPosition,
        out bool requiresTransaction)
    {
        var name = command.TableName;
        var schema = command.Schema;
        var writeOperations = command.ColumnModifications.Where(o => o.IsWrite).ToList();
        var conditionOperations = command.ColumnModifications.Where(o => o.IsCondition).ToList();
        var readOperations = command.ColumnModifications.Where(o => o.IsRead).ToList();

        // Generate plain UPDATE (no RETURNING)
        AppendUpdateCommand(commandStringBuilder, name, schema!, writeOperations, readOperations, conditionOperations, appendReturningOneClause: false);
        commandStringBuilder.AppendLine();

        // Append SELECT ROW_COUNT() for affected rows verification
        commandStringBuilder
            .Append("SELECT ROW_COUNT()")
            .AppendLine(SqlGenerationHelper.StatementTerminator);

        requiresTransaction = false;
        return ResultSetMapping.LastInResultSet;
    }

    public override ResultSetMapping AppendDeleteOperation(
        StringBuilder commandStringBuilder,
        IReadOnlyModificationCommand command,
        int commandPosition,
        out bool requiresTransaction)
    {
        var name = command.TableName;
        var schema = command.Schema;
        var conditionOperations = command.ColumnModifications.Where(o => o.IsCondition).ToList();
        var readOperations = command.ColumnModifications.Where(o => o.IsRead).ToList();

        // Generate plain DELETE (no RETURNING)
        AppendDeleteCommand(commandStringBuilder, name, schema!, readOperations, conditionOperations, appendReturningOneClause: false);
        commandStringBuilder.AppendLine();

        // Append SELECT ROW_COUNT() for affected rows verification
        commandStringBuilder
            .Append("SELECT ROW_COUNT()")
            .AppendLine(SqlGenerationHelper.StatementTerminator);

        requiresTransaction = false;
        return ResultSetMapping.LastInResultSet;
    }

    protected override void AppendReturningClause(
        StringBuilder commandStringBuilder,
        IReadOnlyList<IColumnModification> operations,
        string? additionalValues = null)
    {
        // File-based provider does not support RETURNING clauses
    }

    public override string GenerateNextSequenceValueOperation(string name, string? schema)
        => throw new NotSupportedException($"{GetType().Name}: File-based ADO.NET provider does not support sequences.");
}
