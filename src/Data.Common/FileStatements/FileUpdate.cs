using SqlBuildingBlocks.LogicalEntities;

namespace Data.Common.FileStatements;

public class FileUpdate : FileStatement
{
    public FileUpdate(SqlUpdateDefinition sqlUpdateDefinition, string statement) 
        : base(statement)
    {
        Filter = sqlUpdateDefinition.WhereClause;
        Tables = new SqlTable[] { sqlUpdateDefinition.Table };
        Assignments = sqlUpdateDefinition.Assignments;
    }

    public SqlBinaryExpression? Filter { get; }

    public override IEnumerable<SqlTable> Tables { get; }
    public IList<SqlAssignment> Assignments { get; }

}
