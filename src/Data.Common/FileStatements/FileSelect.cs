using SqlBuildingBlocks.Interfaces;
using SqlBuildingBlocks.LogicalEntities;

namespace Data.Common.FileStatements;

public class FileSelect : FileStatement
{
    public FileSelect(SqlSelectDefinition sqlSelectDefinition, string statement)
        : base(statement)
    {
        SqlSelect = sqlSelectDefinition;
        IsCountQuery = sqlSelectDefinition.Columns.Any(col =>
        {
            if (col is SqlAggregate sqlAggregate && sqlAggregate.AggregateName == "COUNT")
            {
                if (sqlAggregate.Argument != null)
                    ThrowHelper.ThrowIfNotAsterik();
                return true;
            }

            return false;
        });

        Tables = sqlSelectDefinition.Table == null ? null : new SqlTable[] { sqlSelectDefinition.Table };

        if (Tables != null && sqlSelectDefinition.Joins != null)
            Tables = Tables.Concat(sqlSelectDefinition.Joins.Select(join => join.Table));
    }

    public SqlSelectDefinition SqlSelect { get; }
    public override IEnumerable<SqlTable> Tables { get; }
    public IList<ISqlColumn> Columns => SqlSelect.Columns;
    public IList<SqlJoin> Joins => SqlSelect.Joins;
    public SqlLimitOffset Limit => SqlSelect.Limit;


    /// <summary>
    /// Is a column a COUNT(*)?
    /// </summary>
    public bool IsCountQuery { get; }
    
}
