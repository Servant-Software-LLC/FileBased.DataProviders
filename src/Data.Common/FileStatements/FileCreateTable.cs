using SqlBuildingBlocks.LogicalEntities;

namespace Data.Common.FileStatements;

public class FileCreateTable : FileStatement
{
    public FileCreateTable(SqlCreateTableDefinition sqlCreateTableDefinition, string statement)
        : base(statement)
    {
        if (sqlCreateTableDefinition == null)
            throw new ArgumentNullException(nameof(sqlCreateTableDefinition));

        Tables = new List<SqlTable>() { sqlCreateTableDefinition.Table };
        Columns = sqlCreateTableDefinition.Columns;
    }

    /// <summary>
    /// Contains only the new table that will be created.
    /// </summary>
    public override IEnumerable<SqlTable> Tables { get; }

    public IList<SqlColumnDefinition> Columns { get; }
}
