using SqlBuildingBlocks.LogicalEntities;

namespace Data.Common.FileStatements;

public class FileDropColumn : FileStatement
{
    public FileDropColumn(SqlAlterTableDefinition sqlAlterTableDefinition, string statement)
        : base(statement)
    {
        Tables = new SqlTable[] { sqlAlterTableDefinition.Table };
        Columns = sqlAlterTableDefinition.ColumnsToDrop;
    }

    public override IEnumerable<SqlTable> Tables { get; }

    public IList<string> Columns { get; set; }
}
