namespace Data.Common.DataSource;

/// <summary>
/// Provides data for the event that occurs when a data source is changed.
/// </summary>
public class DataSourceEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DataSourceEventArgs"/> class.
    /// </summary>
    /// <param name="table">The name of the table that was changed.</param>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="table"/> is null or empty.</exception>
    public DataSourceEventArgs(string table)
    {
        Table = !string.IsNullOrEmpty(table) ? table : throw new ArgumentNullException(nameof(table));
    }

    /// <summary>
    /// Gets the name of the table that was changed in the data source.
    /// </summary>
    public string Table { get; }
}
