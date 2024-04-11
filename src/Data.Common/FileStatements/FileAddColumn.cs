using SqlBuildingBlocks.LogicalEntities;

namespace Data.Common.FileStatements;

public class FileAddColumn : FileStatement
{
    public FileAddColumn(SqlAlterTableDefinition sqlAlterTableDefinition, string statement)
        : base(statement)
    {
        Tables = new SqlTable[] { sqlAlterTableDefinition.Table };
        Columns = sqlAlterTableDefinition.ColumnsToAdd.Select(def => def.Column).ToList();
    }

    public override IEnumerable<SqlTable> Tables { get; }

    public IList<SqlColumnDefinition> Columns { get; }
}
