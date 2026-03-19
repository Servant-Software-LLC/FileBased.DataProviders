using System.Data.Common;
using System.Data.FileClient;

namespace System.Data.XlsClient;

/// <summary>
/// Automatically generates single-table commands used to reconcile changes made to a
/// <see cref="DataSet"/> with the associated XLS data source.
/// </summary>
public class XlsCommandBuilder : FileCommandBuilder<XlsParameter>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="XlsCommandBuilder"/> class.
    /// </summary>
    public XlsCommandBuilder()
    {
    }

    /// <inheritdoc/>
    protected override void SetRowUpdatingHandler(DbDataAdapter adapter)
    {
        // The XLS provider is read-only and does not support data adapters.
    }
}
