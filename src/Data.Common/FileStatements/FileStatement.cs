using SqlBuildingBlocks.LogicalEntities;

namespace Data.Common.FileStatements;

public abstract class FileStatement
{    
    protected FileStatement(string statement)
    {
        Statement = statement;
    }

    public SqlTable FromTable => Tables != null ? Tables.FirstOrDefault() : null;

    /// <summary>
    /// List of all tables involved in the SQL statement
    /// </summary>
    public abstract IEnumerable<SqlTable> Tables { get; }

    /// <summary>
    /// SQL statement that created this instance.
    /// </summary>
    public string Statement { get; }

}
