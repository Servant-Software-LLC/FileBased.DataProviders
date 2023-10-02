using Microsoft.EntityFrameworkCore.Update;
using System.Text;

namespace EFCore.Xml.Update.Internal;

public class XmlUpdateSqlGenerator : UpdateSqlGenerator
{
    public XmlUpdateSqlGenerator(UpdateSqlGeneratorDependencies dependencies)
    : base(dependencies)
    {
    }

    protected override void AppendIdentityWhereCondition(StringBuilder commandStringBuilder, IColumnModification columnModification)
    {
        if (commandStringBuilder == null)
            throw new ArgumentNullException(nameof(commandStringBuilder));
        if (columnModification == null)
            throw new ArgumentNullException(nameof(columnModification));

        commandStringBuilder.AppendFormat("{0}=LAST_INSERT_ID()", SqlGenerationHelper.DelimitIdentifier(columnModification.ColumnName));
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
            .Append("SELECT ROW_COUNT()")
            .AppendLine(SqlGenerationHelper.StatementTerminator)
            .AppendLine();

        return ResultSetMapping.LastInResultSet;
    }

    protected override void AppendRowsAffectedWhereCondition(StringBuilder commandStringBuilder, int expectedRowsAffected)
    {
        if (commandStringBuilder == null)
            throw new ArgumentNullException(nameof(commandStringBuilder));

        commandStringBuilder
          .Append("ROW_COUNT() = " + expectedRowsAffected)
          .AppendLine();
    }

    public override string GenerateNextSequenceValueOperation(string name, string? schema)
        => throw new NotSupportedException("XML ADO.NET provider does not support sequences.See http://go.microsoft.com/fwlink/?LinkId=723262 for more information.");

}
