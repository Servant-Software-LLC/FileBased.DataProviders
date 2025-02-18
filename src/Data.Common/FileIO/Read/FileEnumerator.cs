using Microsoft.Extensions.Logging;
using SqlBuildingBlocks.POCOs;

namespace Data.Common.FileIO.Read;

/// <summary>
/// Enumerates through the rows of a <see cref="VirtualDataTable"/>, returning each row as an object array.
/// </summary>
/// <remarks>
/// This enumerator is designed to work with a lazy-loaded or virtual data source, where rows are only pulled on-demand.
/// It uses a one-row lookahead mechanism to support the <see cref="MoreRowsAvailable"/> and <see cref="HasRows"/> properties,
/// allowing consumers to check if additional rows are available before attempting to move forward.
/// <para>
/// Note that this enumerator is forward-only and does not support resetting. The <see cref="Reset"/> method will throw a
/// <see cref="NotSupportedException"/> if called.
/// </para>
/// </remarks>
internal class FileEnumerator : IEnumerator<object[]>
{
    private readonly ILogger log;
    private readonly VirtualDataTable virtualDataTable;
    private readonly IEnumerator<DataRow> enumerator;
    private DataRow nextRow;
    private object[] currentRow = Array.Empty<object>();

    public FileEnumerator(VirtualDataTable virtualDataTable, ILogger log)
    {
        this.log = log ?? throw new ArgumentNullException(nameof(log));
        this.virtualDataTable = virtualDataTable ?? throw new ArgumentNullException(nameof(virtualDataTable));

        // Obtain an enumerator from the IEnumerable<DataRow> in VirtualDataTable.
        enumerator = (virtualDataTable.Rows ?? Enumerable.Empty<DataRow>())
                       .GetEnumerator();

        // Pre-fetch the first row if available.
        if (enumerator.MoveNext())
        {
            nextRow = enumerator.Current;
        }
        else
        {
            nextRow = null;
        }

        HasRows = nextRow != null;
    }

    /// <summary>
    /// Returns the current row’s data as an object array.
    /// </summary>
    public object[] Current => currentRow;
    object IEnumerator.Current => Current;

    /// <summary>
    /// Returns the schema of the virtual table. If no columns are defined, returns an empty DataColumnCollection.
    /// </summary>
    public DataColumnCollection Columns => virtualDataTable.Columns ?? new DataTable().Columns;

    /// <summary>
    /// Returns the number of fields (columns) in the table.
    /// </summary>
    public int FieldCount => Columns.Count;

    /// <summary>
    /// Returns the current index of the row that was last fetched.
    /// </summary>
    public int CurrentIndex {  get; private set; }

    /// <summary>
    /// Returns true if there is a next row available (prefetched).
    /// </summary>
    public bool MoreRowsAvailable => nextRow != null;

    /// <summary>
    /// Returns true if there is at least one row available.
    /// </summary>
    public bool HasRows { get; }

    /// <summary>
    /// Advances the enumerator to the next row.
    /// </summary>
    public bool MoveNext()
    {
        log.LogDebug($"FileEnumerator.MoveNext() called. Next row available: {nextRow != null}");

        if (nextRow == null)
        {
            return false;
        }

        // Use the prefetched row as the current row.
        currentRow = nextRow.ItemArray;
        CurrentIndex++;

        // Prefetch the next row.
        if (enumerator.MoveNext())
        {
            nextRow = enumerator.Current;
        }
        else
        {
            nextRow = null;
        }

        return true;
    }

    /// <summary>
    /// Reset is not supported for lazy enumerables.
    /// </summary>
    public void Reset() => throw new NotSupportedException("Reset is not supported for VirtualDataTable-based enumerators.");

    public void Dispose() =>  enumerator.Dispose();

    // Helper methods to mimic DataTable behavior:
    public string GetName(int i) => Columns[i].ColumnName;
    public int GetOrdinal(string name) => Columns.IndexOf(name);
    public Type GetFieldType(int i) => Columns[i].DataType;
}
