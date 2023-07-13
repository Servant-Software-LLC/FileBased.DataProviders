using SqlBuildingBlocks.Interfaces;
using SqlBuildingBlocks.LogicalEntities;
using SqlBuildingBlocks.Utils;

namespace Data.Common.FileStatements;

public class FileInsert : FileStatement
{
    public FileInsert(SqlInsertDefinition sqlInsertDefinition, DbParameterCollection parameters, string statement) 
        : base(sqlInsertDefinition.Table, null, parameters, statement)
    {
        if (sqlInsertDefinition == null)
            throw new ArgumentNullException(nameof(sqlInsertDefinition));

        if (sqlInsertDefinition.Columns.Count != sqlInsertDefinition.Values.Count)
            throw new ArgumentException($"In the INSERT, the number of columns is {sqlInsertDefinition.Columns.Count}, but the number of VALUES is {sqlInsertDefinition.Values.Count}. Their counts must be the same.");

        Columns = sqlInsertDefinition.Columns.Cast<ISqlColumn>().ToList();

        SetValues(sqlInsertDefinition.Values, parameters);
    }

    public override IList<ISqlColumn> Columns { get; }
    public IList<SqlLiteralValue> Values { get; }
    public HashSet<string> ColumnNameHints { get; } = new();

    public IEnumerable<KeyValuePair<string, object>> GetValues()
    {
        var result = Columns.Zip(Values, (name, literalValue) => KeyValuePair.Create(((SqlColumn)name).ColumnName, literalValue.Value));

        return result!;
    }

    // Resolve any parameters in order to set Values property
    private void SetValues(IList<SqlExpression> values, DbParameterCollection parameters)
    {
        List<SqlLiteralValue> sqlLiteralValues = new();
        ResolveParametersVisitor resolveParametersVisitor = new(parameters);
        foreach (var value in values)
        {
            value.Accept(resolveParametersVisitor);

            if (value.Value == null)
                throw new Exception($"SqlExpression value of {value} does not contain a {typeof(SqlLiteralValue)}");

            sqlLiteralValues.Add(value.Value);
        }
    }

}
