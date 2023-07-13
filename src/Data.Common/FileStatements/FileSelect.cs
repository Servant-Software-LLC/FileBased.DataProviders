using Data.Common.FileJoin;
using SqlBuildingBlocks.Interfaces;
using SqlBuildingBlocks.LogicalEntities;

namespace Data.Common.FileStatements;

public class FileSelect : FileStatement
{
    private readonly IList<SqlJoin> sqlJoins;

    public FileSelect(SqlSelectDefinition sqlSelectDefinition, DbParameterCollection parameters, string statement)
        : base(sqlSelectDefinition.Table, sqlSelectDefinition.WhereClause, parameters, statement)
    {
        Limit = sqlSelectDefinition.Limit;
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
        Columns = sqlSelectDefinition.Columns;
        sqlJoins = sqlSelectDefinition.Joins;
    }

    public SqlLimitOffset Limit { get; }

    public override IList<ISqlColumn> Columns { get; }

    /// <summary>
    /// Is a column a COUNT(*)?
    /// </summary>
    public bool IsCountQuery { get; }
    
    public DataTableJoin GetFileJoin() => new DataTableJoin(sqlJoins, Table.TableName);

}
