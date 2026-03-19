using System.Data.Common;
using System.Data.FileClient;

namespace System.Data.XmlClient;

/// <summary>
/// Automatically generates single-table commands used to reconcile changes made to a
/// <see cref="DataSet"/> with the associated XML data source.
/// </summary>
public class XmlCommandBuilder : FileCommandBuilder<XmlParameter>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="XmlCommandBuilder"/> class.
    /// </summary>
    public XmlCommandBuilder()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="XmlCommandBuilder"/> class
    /// with the specified <see cref="XmlDataAdapter"/>.
    /// </summary>
    /// <param name="adapter">The <see cref="XmlDataAdapter"/> to generate commands for.</param>
    public XmlCommandBuilder(XmlDataAdapter adapter) : base(adapter)
    {
    }

    /// <inheritdoc/>
    protected override void SetRowUpdatingHandler(DbDataAdapter adapter)
    {
        // The FileDataAdapter does not currently expose RowUpdating/RowUpdated events.
        // Command generation via GetInsertCommand/GetUpdateCommand/GetDeleteCommand still works.
    }
}
