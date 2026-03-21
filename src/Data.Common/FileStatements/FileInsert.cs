using SqlBuildingBlocks.Interfaces;
using SqlBuildingBlocks.LogicalEntities;

namespace Data.Common.FileStatements;

public class FileInsert : FileStatement
{
    private readonly IList<ISqlColumn> columns;

    public FileInsert(SqlInsertDefinition sqlInsertDefinition, string statement) 
        : base(statement)
    {
        if (sqlInsertDefinition == null)
            throw new ArgumentNullException(nameof(sqlInsertDefinition));

        if (sqlInsertDefinition.Values.Count == 0)
            throw new ArgumentException("INSERT statement must have at least one row of values.");

        var firstRow = sqlInsertDefinition.Values[0];
        if (sqlInsertDefinition.Columns.Count != firstRow.Count)
            throw new ArgumentException($"In the INSERT, the number of columns is {sqlInsertDefinition.Columns.Count}, but the number of VALUES is {firstRow.Count}. Their counts must be the same.");

        Tables = new SqlTable[] { sqlInsertDefinition.Table };
        columns = sqlInsertDefinition.Columns.Cast<ISqlColumn>().ToList();
        SetValues(firstRow);
    }

    public override IEnumerable<SqlTable> Tables { get; }
    public IList<SqlLiteralValue> Values { get; private set; }
    public HashSet<string> ColumnNameHints { get; } = new();

    public IDictionary<string,object> GetValues()
    {
        var result = columns.Zip(Values, (name, literalValue) => new { ((SqlColumn)name).ColumnName, literalValue.Value });

        return result!.ToDictionary(item => item.ColumnName, item => item.Value);
    }

    private void SetValues(IList<SqlExpression> values)
    {
        List<SqlLiteralValue> sqlLiteralValues = new();
        foreach (var value in values)
        {
            if (value.Value == null)
                throw new Exception($"SqlExpression value of {value} does not contain a {typeof(SqlLiteralValue)}");

            sqlLiteralValues.Add(value.Value);
        }

        Values = sqlLiteralValues;
    }

}
