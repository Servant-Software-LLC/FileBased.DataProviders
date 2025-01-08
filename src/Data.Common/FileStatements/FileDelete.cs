using SqlBuildingBlocks.LogicalEntities;

namespace Data.Common.FileStatements;

public class FileDelete : FileStatement, IContainsReturning
{
    public FileDelete(SqlDeleteDefinition sqlDeleteDefinition, string statement) 
        : base(statement)
    {
        Filter = sqlDeleteDefinition.WhereClause;
        Tables = new SqlTable[] { sqlDeleteDefinition.Table };

        if (sqlDeleteDefinition.Returning != null)
        {
            if (sqlDeleteDefinition.Returning.Int == null || sqlDeleteDefinition.Returning.Int != 1)
                throw new NotSupportedException("The RETURNING clause in an DELETE can only be used with an interger value.");

            Returning = sqlDeleteDefinition.Returning.Int.Value;
        }
    }

    public SqlBinaryExpression Filter { get; }

    public override IEnumerable<SqlTable> Tables { get; }

    public int? Returning { get; }
}
