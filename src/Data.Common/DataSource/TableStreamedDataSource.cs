using Data.Common.Utils;
using System.Text;

namespace Data.Common.DataSource;

/// <summary>
/// Provides a data source implementation that stores table data in memory using streams. 
/// This is useful for scenarios where data is transient or does not need to be persisted to the file system.
/// </summary>
public class TableStreamedDataSource : StreamedDataSource
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TableStreamedDataSource"/> class.
    /// </summary>
    public TableStreamedDataSource(string database) : base(database) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="TableStreamedDataSource"/> class.
    /// </summary>
    /// <param name="tableName">The name of the initial table.</param>
    /// <param name="utf8TableData">The stream containing UTF-8 encoded data for the initial table.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when either <paramref name="tableName"/> or <paramref name="utf8TableData"/> is null.
    /// </exception>
    public TableStreamedDataSource(string database, string tableName, Stream stream)
        : base(database)
    {
        AddTable(tableName, stream);
    }

    public TableStreamedDataSource(string database, string tableName, string tableContent)
        : base(database)
    {
        AddTable(tableName, tableContent);
    }

    public TableStreamedDataSource(string database, string tableName, Func<Stream> streamCreationFunc)
        : base(database)
    {
        AddTable(tableName, streamCreationFunc);
    }

    /// <summary>
    /// Adds a new table and its associated data stream to the data source.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <param name="utf8TableData">The stream containing UTF-8 encoded data for the table.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when either <paramref name="tableName"/> or <paramref name="utf8TableData"/> is null.
    /// </exception>
    public void AddTable(string tableName, Func<Stream> streamCreationFunc)
    {
        tables.AddTable(tableName, streamCreationFunc);
        OnChanged(tableName);
    }

    public void AddTable(string tableName, Stream stream) => AddTable(tableName, ()=>stream);

    public void AddTable(string tableName, string tableContent)
    {
        if (string.IsNullOrEmpty(tableContent))
            throw new ArgumentNullException(nameof(tableContent));

        byte[] fileBytes = Encoding.UTF8.GetBytes(tableContent);
        MemoryStream fileStream = new MemoryStream(fileBytes);
        AddTable(tableName, fileStream);
    }
}
