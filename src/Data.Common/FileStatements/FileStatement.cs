using SqlBuildingBlocks.Interfaces;
using SqlBuildingBlocks.LogicalEntities;

namespace Data.Common.FileStatements;

public abstract class FileStatement
{    
    protected FileStatement(SqlBinaryExpression filter, string statement)
    {
        Filter = filter;
        Statement = statement;
    }

    public SqlTable FromTable => Tables != null ? Tables.FirstOrDefault() : null;
    public SqlBinaryExpression? Filter { get; }

    /// <summary>
    /// List of all tables involved in the SQL statement
    /// </summary>
    public abstract IEnumerable<SqlTable> Tables { get; }

    /// <summary>
    /// SQL statement that created this instance.
    /// </summary>
    public string Statement { get; }


    //TODO:  Do we need this (abstract) property at this level?
    public abstract IList<ISqlColumn> Columns { get; }
}
