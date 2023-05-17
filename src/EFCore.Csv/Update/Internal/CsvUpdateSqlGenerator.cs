using Microsoft.EntityFrameworkCore.Update;
using System.Text;

namespace EFCore.Csv.Update.Internal;

public class CsvUpdateSqlGenerator : UpdateSqlGenerator
{
    public CsvUpdateSqlGenerator(UpdateSqlGeneratorDependencies dependencies)
    : base(dependencies)
    {
    }

    protected override void AppendIdentityWhereCondition(StringBuilder commandStringBuilder, IColumnModification columnModification)
    {
        if (commandStringBuilder == null)
            throw new ArgumentNullException(nameof(commandStringBuilder));
        if (columnModification == null)
            throw new ArgumentNullException(nameof(columnModification));

        SqlGenerationHelper.DelimitIdentifier(commandStringBuilder, "rowid");
        commandStringBuilder.Append(" = ")
            .Append("last_insert_rowid()");
    }

    protected override ResultSetMapping AppendSelectAffectedCountCommand(
    StringBuilder commandStringBuilder,
    string name,
    string? schema,
    int commandPosition)
    {
        if (commandStringBuilder == null)
            throw new ArgumentNullException(nameof(commandStringBuilder));
        if (string.IsNullOrEmpty(name)) 
            throw new ArgumentNullException(nameof(name));

        commandStringBuilder
            .Append("SELECT changes()")
            .AppendLine(SqlGenerationHelper.StatementTerminator)
            .AppendLine();

        return ResultSetMapping.LastInResultSet;
    }

    protected override void AppendRowsAffectedWhereCondition(StringBuilder commandStringBuilder, int expectedRowsAffected)
    {
        if (commandStringBuilder == null)
            throw new ArgumentNullException(nameof(commandStringBuilder));

        commandStringBuilder.Append("changes() = ").Append(expectedRowsAffected);
    }

    public override string GenerateNextSequenceValueOperation(string name, string? schema)
        => throw new NotSupportedException("SQLite does not support sequences.See http://go.microsoft.com/fwlink/?LinkId=723262 for more information.");

}
