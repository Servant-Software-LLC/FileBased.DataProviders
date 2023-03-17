namespace Data.Xml.JsonJoin;
public class Join
{
    public string TableName { get; internal set; }
    public string JoinColumn { get; internal set; }
    public string Operation { get; } = "=";
    public string SourceColumn { get; internal set; }
    public IList<Join> InnerJoin { get; }

    public Join(string tableName, string joinColumn, string sourceColumn)
    {
        if (string.IsNullOrEmpty(tableName))
        {
            throw new ArgumentException($"'{nameof(tableName)}' cannot be null or empty.", nameof(tableName));
        }

        if (string.IsNullOrEmpty(joinColumn))
        {
            throw new ArgumentException($"'{nameof(joinColumn)}' cannot be null or empty.", nameof(joinColumn));
        }

        if (string.IsNullOrEmpty(sourceColumn))
        {
            throw new ArgumentException($"'{nameof(sourceColumn)}' cannot be null or empty.", nameof(sourceColumn));
        }

        InnerJoin = new List<Join>();
        TableName = tableName;
        JoinColumn = joinColumn;
        SourceColumn = sourceColumn;
    }

}
