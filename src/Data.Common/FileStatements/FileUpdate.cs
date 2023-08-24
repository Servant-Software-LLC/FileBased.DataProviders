using SqlBuildingBlocks.Interfaces;
using SqlBuildingBlocks.LogicalEntities;

namespace Data.Common.FileStatements;

public class FileUpdate : FileStatement
{
    public FileUpdate(SqlUpdateDefinition sqlUpdateDefinition, string statement) 
        : base(sqlUpdateDefinition.WhereClause, statement)
    {
        Tables = new SqlTable[] { sqlUpdateDefinition.Table };
        Assignments = sqlUpdateDefinition.Assignments;
    }

    public override IEnumerable<SqlTable> Tables { get; }
    public IList<SqlAssignment> Assignments { get; }

    public override IList<ISqlColumn> Columns => throw new NotImplementedException();

}
