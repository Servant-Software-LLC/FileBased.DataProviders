using Data.Common.Utils.ConnectionString;
using Data.Xml.XmlIO;
using System.Xml.Linq;

namespace System.Data.XmlClient;

/// <summary>
/// Represents a connection to an XML file that can be used to execute commands against the file.
/// </summary>
public class XmlConnection : FileConnection<XmlParameter>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="XmlConnection"/> class.
    /// </summary>
    public XmlConnection() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="XmlConnection"/> class with a connection string.
    /// </summary>
    /// <param name="connectionString">The connection string used to connect to the XML file.</param>
    public XmlConnection(string connectionString)
        : this(new FileConnectionString(connectionString)) { }

    public XmlConnection(FileConnectionString connectionString)
        : base(connectionString)
    {
        FileReader = !AdminMode ? new XmlReader(this) : null;
    }

    /// <summary>
    /// Gets the file extension for the XML file.
    /// </summary>
    public override string FileExtension => "xml";


#if NET7_0_OR_GREATER    // .NET 7 implementation that supports covariant return types

    /// <summary>
    /// Begins a new transaction.
    /// </summary>
    /// <returns>A new <see cref="XmlTransaction"/> object.</returns>
    public override XmlTransaction BeginTransaction() => new(this, default);

    /// <summary>
    /// Begins a new transaction with the specified isolation level.
    /// </summary>
    /// <param name="il">The isolation level of the transaction.</param>
    /// <returns>A new <see cref="XmlTransaction"/> object.</returns>
    public override XmlTransaction BeginTransaction(IsolationLevel il) => BeginTransaction();

    /// <summary>
    /// Begins a new database transaction.
    /// </summary>
    /// <param name="isolationLevel">The isolation level of the transaction.</param>
    /// <returns>A new <see cref="XmlTransaction"/> object.</returns>
    protected override XmlTransaction BeginDbTransaction(IsolationLevel isolationLevel) => BeginTransaction(isolationLevel);

    /// <summary>
    /// Creates a new data adapter with the specified query.
    /// </summary>
    /// <param name="query">The query to use for the data adapter.</param>
    /// <returns>A new <see cref="XmlDataAdapter"/> object.</returns>
    public override XmlDataAdapter CreateDataAdapter(string query) => new(query, this);

    /// <summary>
    /// Creates a new command object.
    /// </summary>
    /// <returns>A new <see cref="XmlCommand"/> object.</returns>
    public override XmlCommand CreateCommand() => new(this);

    /// <summary>
    /// Creates a new command object with the specified command text.
    /// </summary>
    /// <param name="cmdText">The command text to use for the command object.</param>
    /// <returns>A new <see cref="XmlCommand"/> object.</returns>
    public override XmlCommand CreateCommand(string cmdText) => new(cmdText, this);

#else       // .NET Standard 2.0 compliant implementation.

    /// <summary>
    /// Begins a new transaction.
    /// </summary>
    /// <returns>A new <see cref="XmlTransaction"/> object.</returns>
    public override FileTransaction<XmlParameter> BeginTransaction() => new XmlTransaction(this, default);

    /// <summary>
    /// Begins a new transaction with the specified isolation level.
    /// </summary>
    /// <param name="il">The isolation level of the transaction.</param>
    /// <returns>A new <see cref="XmlTransaction"/> object.</returns>
    public override FileTransaction<XmlParameter> BeginTransaction(IsolationLevel il) => BeginTransaction();

    /// <summary>
    /// Begins a new database transaction.
    /// </summary>
    /// <param name="isolationLevel">The isolation level of the transaction.</param>
    /// <returns>A new <see cref="XmlTransaction"/> object.</returns>
    protected override Common.DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) => BeginTransaction(isolationLevel);

    /// <summary>
    /// Creates a new data adapter with the specified query.
    /// </summary>
    /// <param name="query">The query to use for the data adapter.</param>
    /// <returns>A new <see cref="XmlDataAdapter"/> object.</returns>
    public override FileDataAdapter<XmlParameter> CreateDataAdapter(string query) => new XmlDataAdapter(query, this);

    /// <summary>
    /// Creates a new command object.
    /// </summary>
    /// <returns>A new <see cref="XmlCommand"/> object.</returns>
    public override FileCommand<XmlParameter> CreateCommand() => new XmlCommand(this);

    /// <summary>
    /// Creates a new command object with the specified command text.
    /// </summary>
    /// <param name="cmdText">The command text to use for the command object.</param>
    /// <returns>A new <see cref="XmlCommand"/> object.</returns>
    public override FileCommand<XmlParameter> CreateCommand(string cmdText) => new XmlCommand(cmdText, this);

#endif

    /// <summary>
    /// Creates the XML file as a database.
    /// </summary>
    /// <param name="databaseFileName">The name of the database file to create.</param>
    protected override void CreateFileAsDatabase(string databaseFileName)
    {
        var doc = new XDocument(new XElement("DataSet"));
        doc.Save(databaseFileName);
    }

    /// <summary>
    /// Creates a new data set writer.
    /// </summary>
    protected override Func<FileStatement, IDataSetWriter> CreateDataSetWriter => fileStatement => new XmlDataSetWriter(this, fileStatement);
}
