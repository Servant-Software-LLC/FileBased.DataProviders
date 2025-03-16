using Data.Common.DataSource;
using Data.Common.Utils.ConnectionString;

namespace System.Data.XlsClient;

/// <summary>
/// Represents a connection for XLS operations.
/// </summary>
/// <inheritdoc/>
public class XlsConnection : FileConnection<XlsParameter>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="XlsConnection"/> class.
    /// </summary>
    public XlsConnection() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="XlsConnection"/> class with the specified connection string.
    /// </summary>
    /// <param name="connectionString">The connection string to use.</param>
    public XlsConnection(string connectionString)
        : this(new FileConnectionString(connectionString)) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="XlsConnection"/> class with the specified connection string.
    /// </summary>
    /// <param name="connectionString">The connection string to use.</param>
    public XlsConnection(FileConnectionString connectionString)
        : base(connectionString) { }

    /// <inheritdoc />
    public override string FileExtension => "xls";

    public Func<IEnumerable<string>, Type> GuessTypeFunction { get; set; }
    public int GuessTypeRows { get; set; } = 1000;

    protected override FileReader CreateFileReader => new XlsReader(this);

    /// <inheritdoc />
    public override void Open()
    {
        base.Open();
        EnsureNotFolderAsDatabase();
    }

    /// <inheritdoc />
    public override void ChangeDatabase(string connString)
    {
        base.ChangeDatabase(connString);
        EnsureNotFolderAsDatabase();
    }

    /// <inheritdoc/>
    private void EnsureNotFolderAsDatabase()
    {
        if (DataSourceProvider == null && DataSourceType == DataSourceType.Directory)
        {
            throw new InvalidOperationException("Folder as database is not supported in Xls Provider");
        }
    }

    /// <inheritdoc/>
    public override FileTransaction<XlsParameter> BeginTransaction() => throw new InvalidOperationException("The XSL provider doesn't support transactions.");

    /// <inheritdoc/>
    public override FileTransaction<XlsParameter> BeginTransaction(IsolationLevel il) => BeginTransaction();

    /// <inheritdoc/>
    protected override Common.DbTransaction BeginDbTransaction(IsolationLevel il) => BeginTransaction(il);

    /// <inheritdoc/>
    public override IFileDataAdapter CreateDataAdapter(string query) => throw new InvalidOperationException("The XSL provider doesn't support adapters.");


#if NET7_0_OR_GREATER    // .NET 7 implementation that supports covariant return types
    /* TODO: If ever we support, transactions and/or the adapter, then move the overrides above into the #else and uncomment this.
        /// <inheritdoc/>
        public override XlsTransaction BeginTransaction() => new(this, default);

        /// <inheritdoc/>
        public override XlsTransaction BeginTransaction(IsolationLevel il) => BeginTransaction();

        /// <inheritdoc/>
        protected override XlsTransaction BeginDbTransaction(IsolationLevel il) => BeginTransaction(il);

        /// <inheritdoc/>
        public override XlsDataAdapter CreateDataAdapter(string query) => new(query, this);
    */

    /// <inheritdoc/>
    public override XlsCommand CreateCommand() => new(this);

    /// <inheritdoc/>
    public override XlsCommand CreateCommand(string cmdText) => new(cmdText, this);

#else       // .NET Standard 2.0 compliant implementation.

    /// <inheritdoc/>
    public override FileCommand<XlsParameter> CreateCommand() => new XlsCommand(this);

    /// <inheritdoc/>
    public override FileCommand<XlsParameter> CreateCommand(string cmdText) => new XlsCommand(cmdText, this);

#endif

    /// <inheritdoc/>
    protected override void CreateFileAsDatabase(string databaseFileName) =>
        //TODO: Maltby - Is this good enough?  Or must an XLS contain some basic content.
        File.WriteAllText(databaseFileName, string.Empty);

    /// <inheritdoc/>
    protected override Func<FileStatement, IDataSetWriter> CreateDataSetWriter => 
        throw new InvalidOperationException("The XLS provider does not support writing at this time.");

}
