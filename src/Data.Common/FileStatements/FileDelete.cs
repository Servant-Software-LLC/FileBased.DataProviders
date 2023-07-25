using SqlBuildingBlocks.Interfaces;
using SqlBuildingBlocks.LogicalEntities;

namespace Data.Common.FileStatements;

public class FileDelete : FileStatement
{
    public FileDelete(SqlDeleteDefinition sqlDeleteDefinition, string statement) 
        : base(sqlDeleteDefinition.WhereClause, statement)
    {
        Tables = new SqlTable[] { sqlDeleteDefinition.Table };
    }

    public override IEnumerable<SqlTable> Tables { get; }

    public override IList<ISqlColumn> Columns => throw new NotImplementedException();
}
