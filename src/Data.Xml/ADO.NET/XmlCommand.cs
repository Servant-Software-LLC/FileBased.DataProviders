using Data.Common.Utils;
using Data.Xml.XmlIO.SchemaAltering;

namespace System.Data.XmlClient;


/// <summary>
/// Represents an XML command that is used to execute stored procedures and other commands at a data source.
/// </summary>
public class XmlCommand : FileCommand<XmlParameter>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="XmlCommand"/> class.
    /// </summary>
    public XmlCommand() { }

    public XmlCommand(string command) : base(command) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="XmlCommand"/> class with the specified command text.
    /// </summary>
    /// <param name="command">The text of the command.</param>
    public XmlCommand(XmlConnection connection) : base(connection) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="XmlCommand"/> class with the specified connection.
    /// </summary>
    /// <param name="connection">The connection to the data source.</param>
    public XmlCommand(string cmdText, XmlConnection connection) : base(cmdText, connection) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="XmlCommand"/> class with the specified command text and connection.
    /// </summary>
    /// <param name="cmdText">The text of the command.</param>
    /// <param name="connection">The connection to the data source.</param>
    public XmlCommand(string cmdText, XmlConnection connection, XmlTransaction transaction)
        : base(cmdText, connection, transaction)
    {
    }

    /// <inheritdoc/>
    public override XmlParameter CreateParameter() => new();

    /// <inheritdoc/>
    public override XmlParameter CreateParameter(string parameterName, object value) =>
        value is DbType dbType ? new(parameterName, dbType) : new(parameterName, value);

    /// <inheritdoc/>
    protected override FileWriter CreateWriter(FileStatement fileStatement) => fileStatement switch
    {
        FileDelete deleteStatement => new XmlDelete(deleteStatement, (XmlConnection)Connection!, this),
        FileInsert insertStatement => new XmlInsert(insertStatement, (XmlConnection)Connection!, this),
        FileUpdate updateStatement => new XmlUpdate(updateStatement, (XmlConnection)Connection!, this),
        FileCreateTable createTableStatement => new XmlCreateTable(createTableStatement, (XmlConnection)Connection!, this),
        FileDropColumn dropTableStatement => new XmlDropColumn(dropTableStatement, (XmlConnection)Connection!, this),
        FileAddColumn addColumnStatement => new XmlAddColumn(addColumnStatement, (XmlConnection)Connection!, this),

        _ => throw new InvalidOperationException("query not supported")
    };

#if NET7_0_OR_GREATER    // .NET 7 implementation that supports covariant return types

    /// <inheritdoc/>
    public override XmlDataAdapter CreateAdapter() => new(this);


    /// <inheritdoc/>
    protected override XmlDataReader CreateDataReader(IEnumerable<FileStatement> fileStatements, LoggerServices loggerServices) =>
        new(fileStatements, FileConnection.FileReader, FileTransaction == null ? null : FileTransaction.TransactionScopedRows, CreateWriter, loggerServices);

#else       // .NET Standard 2.0 compliant implementation.

    /// <inheritdoc/>
    public override FileDataAdapter<XmlParameter> CreateAdapter() => new XmlDataAdapter(this);


    /// <inheritdoc/>
    protected override FileDataReader CreateDataReader(IEnumerable<FileStatement> fileStatements, LoggerServices loggerServices) =>
        new XmlDataReader(fileStatements, FileConnection.FileReader, FileTransaction == null ? null : FileTransaction.TransactionScopedRows, CreateWriter, loggerServices);

#endif
}
