namespace System.Data.XmlClient;


/// <summary>
/// Represents a set of data commands and a database connection that are used to fill the DataSet and update an XML file.
/// </summary>
public class XmlDataAdapter : FileDataAdapter<XmlParameter>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="XmlDataAdapter"/> class with no arguments.
    /// </summary>
    public XmlDataAdapter() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="XmlDataAdapter"/> class with a select command.
    /// </summary>
    /// <param name="selectCommand">The select command to use for the data adapter.</param>
    public XmlDataAdapter(XmlCommand selectCommand) : base(selectCommand) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="XmlDataAdapter"/> class with a query and a connection.
    /// </summary>
    /// <param name="query">The query to use for the data adapter.</param>
    /// <param name="connection">The connection to use for the data adapter.</param>
    public XmlDataAdapter(string query, XmlConnection connection) : base(query, connection) { }

    /// <summary>
    /// Creates a new instance of the <see cref="FileWriter"/> class based on the specified file statement.
    /// </summary>
    /// <param name="fileStatement">The file statement that determines the type of writer to create.</param>
    /// <returns>The created <see cref="FileWriter"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the query type is not supported.</exception>
    protected override FileWriter CreateWriter(FileStatement fileStatement) => fileStatement switch
    {
        global::Data.Common.FileStatements.FileDelete deleteStatement => new XmlDelete(deleteStatement, (XmlConnection)UpdateCommand!.Connection!, (FileCommand<XmlParameter>)UpdateCommand),
        global::Data.Common.FileStatements.FileInsert insertStatement => new XmlInsert(insertStatement, (XmlConnection)UpdateCommand!.Connection!, (FileCommand<XmlParameter>)UpdateCommand),
        global::Data.Common.FileStatements.FileUpdate updateStatement => new XmlUpdate(updateStatement, (XmlConnection)UpdateCommand!.Connection!, (FileCommand<XmlParameter>)UpdateCommand),

        _ => throw new InvalidOperationException("query not supported")
    };
}
