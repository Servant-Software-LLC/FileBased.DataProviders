using System.Data.Common;
using System.Data.FileClient;

namespace System.Data.CsvClient;

/// <summary>
/// Automatically generates single-table commands used to reconcile changes made to a
/// <see cref="DataSet"/> with the associated CSV data source.
/// </summary>
public class CsvCommandBuilder : FileCommandBuilder<CsvParameter>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CsvCommandBuilder"/> class.
    /// </summary>
    public CsvCommandBuilder()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CsvCommandBuilder"/> class
    /// with the specified <see cref="CsvDataAdapter"/>.
    /// </summary>
    /// <param name="adapter">The <see cref="CsvDataAdapter"/> to generate commands for.</param>
    public CsvCommandBuilder(CsvDataAdapter adapter) : base(adapter)
    {
    }

    /// <inheritdoc/>
    protected override void SetRowUpdatingHandler(DbDataAdapter adapter)
    {
        // The FileDataAdapter does not currently expose RowUpdating/RowUpdated events.
        // Command generation via GetInsertCommand/GetUpdateCommand/GetDeleteCommand still works.
    }
}
