using SqlBuildingBlocks.Interfaces;
using SqlBuildingBlocks.LogicalEntities;

namespace Data.Common.FileStatements;

public class FileDelete : FileStatement
{
    public FileDelete(SqlDeleteDefinition sqlDeleteDefinition, DbParameterCollection parameters, string statement) 
        : base(sqlDeleteDefinition.Table, sqlDeleteDefinition.WhereClause, parameters, statement)
    {
    }

    public override IList<ISqlColumn> Columns => throw new NotImplementedException();
}
