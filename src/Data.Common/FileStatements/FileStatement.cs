using SqlBuildingBlocks.Interfaces;
using SqlBuildingBlocks.LogicalEntities;

namespace Data.Common.FileStatements;

public abstract class FileStatement
{    
    protected FileStatement(SqlTable sqlTable, SqlBinaryExpression filter, DbParameterCollection parameters, string statement)
    {
        Table = sqlTable;
        Parameters = parameters;
        Filter = filter;
        Statement = statement;
    }

    protected SqlTable Table { get; }
    public string TableName => Table.TableName;
    public SqlBinaryExpression? Filter { get; }

    /// <summary>
    /// SQL statement that created this instance.
    /// </summary>
    public string Statement { get; }
    public DbParameterCollection Parameters { get; }

    public abstract IList<ISqlColumn> Columns { get; }
}
