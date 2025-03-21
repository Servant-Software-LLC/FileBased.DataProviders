using Data.Common.DataSource;
using Data.Common.Utils.ConnectionString;
using Data.Xls.DataSource;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;

namespace System.Data.XlsClient;

/// <summary>
/// Represents a connection for XLS operations.
/// </summary>
/// <inheritdoc/>
public class XlsConnection : FileConnection<XlsParameter>, IDisposable
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
        : base(connectionString) 
    { 
        //NOTE:  This is a bit of a workaround for now.  This provider has been built differently in regards to the File/FolderAsDatabase concept.
        //       Until now, the stream data sources only supported the FolderAsDatabase concept.  When an XLS file path is provided, this code
        //       here, splits it into streams/table to mimic a FolderAsDatabase approach for the streamed data source.  When/if this gets reworked
        //       to properly support FileAsDatabase streamed data sources, then this workaround can go away.
        if (DataSourceType == DataSourceType.File)
        {
            var databaseFile = DataSource;
            var streamXlsFile = File.OpenRead(databaseFile);
            var dataSourceProvider = new XlsStreamedDataSource(databaseFile, streamXlsFile);
            DataSourceProvider = dataSourceProvider;

            
            ConnectionString = FileConnectionString.CustomDataSource;
        }
    }

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
        CreateEmptyXlsx(databaseFileName);

    /// <inheritdoc/>
    protected override Func<FileStatement, IDataSetWriter> CreateDataSetWriter => 
        throw new InvalidOperationException("The XLS provider does not support writing at this time.");

    private static void CreateEmptyXlsx(string filePath)
    {
        // Create a new SpreadsheetDocument (XLSX) in read/write mode.
        using (SpreadsheetDocument document = SpreadsheetDocument.Create(filePath, SpreadsheetDocumentType.Workbook))
        {
            // Add a WorkbookPart to the document.
            WorkbookPart workbookPart = document.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            // Add a WorksheetPart to the WorkbookPart.
            WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            // Create an empty worksheet with a SheetData element.
            worksheetPart.Worksheet = new Worksheet(new SheetData());

            // Create the Sheets collection and add it to the workbook.
            Sheets sheets = document.WorkbookPart.Workbook.AppendChild(new Sheets());

            // Append a new Sheet to the Sheets collection.
            Sheet sheet = new Sheet()
            {
                Id = document.WorkbookPart.GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = "Sheet1"
            };
            sheets.Append(sheet);

            // Save the workbook.
            workbookPart.Workbook.Save();
        }
    }

    void IDisposable.Dispose() 
    {
        if (DataSourceProvider != null && DataSourceProvider is IDisposable disposableDataSourceProvider)
        {
            disposableDataSourceProvider.Dispose();
        }
    }
}
