using SqlBuildingBlocks.Interfaces;
using SqlBuildingBlocks.LogicalEntities;

namespace Data.Common.FileStatements;

public class FileDelete : FileStatement
{
    public FileDelete(SqlDeleteDefinition sqlDeleteDefinition, string statement) 
        : base(statement)
    {
        Filter = sqlDeleteDefinition.WhereClause;
        Tables = new SqlTable[] { sqlDeleteDefinition.Table };
    }

    public SqlBinaryExpression? Filter { get; }

    public override IEnumerable<SqlTable> Tables { get; }
}
