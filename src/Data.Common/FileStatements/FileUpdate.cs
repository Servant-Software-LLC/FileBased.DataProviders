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

        if (sqlUpdateDefinition.Returning != null)
        {
            if (sqlUpdateDefinition.Returning.Int == null || sqlUpdateDefinition.Returning.Int != 1)
                throw new NotSupportedException("The RETURNING clause in an UPDATE can only be used with an interger value.");

            Returning = sqlUpdateDefinition.Returning.Int.Value;
        }
    }

    public SqlBinaryExpression? Filter { get; }

    public override IEnumerable<SqlTable> Tables { get; }
    public IList<SqlAssignment> Assignments { get; }

    public int? Returning { get; }

}
