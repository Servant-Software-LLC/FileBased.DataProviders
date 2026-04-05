using System.Data.Common;
using System.Data.FileClient;

namespace System.Data.JsonClient;

/// <summary>
/// Automatically generates single-table commands used to reconcile changes made to a
/// <see cref="DataSet"/> with the associated JSON data source.
/// </summary>
public class JsonCommandBuilder : FileCommandBuilder<JsonParameter>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JsonCommandBuilder"/> class.
    /// </summary>
    public JsonCommandBuilder()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonCommandBuilder"/> class
    /// with the specified <see cref="JsonDataAdapter"/>.
    /// </summary>
    /// <param name="adapter">The <see cref="JsonDataAdapter"/> to generate commands for.</param>
    public JsonCommandBuilder(JsonDataAdapter adapter) : base(adapter)
    {
    }

    /// <inheritdoc/>
    protected override void SetRowUpdatingHandler(DbDataAdapter adapter)
    {
        // The FileDataAdapter does not currently expose RowUpdating/RowUpdated events.
        // Command generation via GetInsertCommand/GetUpdateCommand/GetDeleteCommand still works.
    }
}
