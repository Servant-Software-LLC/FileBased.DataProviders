using SqlBuildingBlocks.LogicalEntities;

namespace Data.Common.FileStatements;

public class FileDropTable : FileStatement
{
    public FileDropTable(string tableName, string statement)
        : base(statement)
    {
        if (string.IsNullOrEmpty(tableName))
            throw new ArgumentNullException(nameof(tableName));

        Tables = new List<SqlTable>() { new SqlTable(null, tableName) };
    }

    public override IEnumerable<SqlTable> Tables { get; }
}
