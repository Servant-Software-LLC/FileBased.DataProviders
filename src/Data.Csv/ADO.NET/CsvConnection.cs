using Data.Common.Utils.ConnectionString;
using Data.Csv.CsvIO;

namespace System.Data.CsvClient;


/// <summary>
/// Represents a connection for CSV operations.
/// </summary>
/// <inheritdoc/>
public class CsvConnection : FileConnection<CsvParameter>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CsvConnection"/> class with the specified connection string.
    /// </summary>
    /// <param name="connectionString">The connection string to use.</param>
    public CsvConnection(string connectionString)
        : this(new FileConnectionString(connectionString)) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="CsvConnection"/> class with the specified connection string.
    /// </summary>
    /// <param name="connectionString">The connection string to use.</param>
    public CsvConnection(FileConnectionString connectionString)
        : base(connectionString)
    {
        FileReader = !AdminMode ? new CsvReader(this) : null;
    }

    /// <inheritdoc />
    public override string FileExtension => "csv";

    /// <inheritdoc />
    public override void Open()
    {
        base.Open();

        EnsureNotFileAsDatabase();
    }

    /// <inheritdoc />
    public override void ChangeDatabase(string connString)
    {
        base.ChangeDatabase(connString);
        EnsureNotFileAsDatabase();
    }

    /// <inheritdoc/>
    private void EnsureNotFileAsDatabase()
    {
        if (PathType == PathType.File)
        {
            throw new InvalidOperationException("File as database is not supported in Csv Provider");

        }
    }

    /// <inheritdoc/>
    public override CsvTransaction BeginTransaction() => new(this, default);

    /// <inheritdoc/>
    public override CsvTransaction BeginTransaction(IsolationLevel il) => BeginTransaction();

    /// <inheritdoc/>
    public override CsvDataAdapter CreateDataAdapter(string query) => new(query, this);

    /// <inheritdoc/>
    public override CsvCommand CreateCommand() => new(this);

    /// <inheritdoc/>
    public override CsvCommand CreateCommand(string cmdText) => new(cmdText, this);

    /// <inheritdoc/>
    protected override void CreateFileAsDatabase(string databaseFileName) =>
        File.WriteAllText(databaseFileName, string.Empty);

    /// <inheritdoc/>
    protected override Func<FileStatement, IDataSetWriter> CreateDataSetWriter => fileStatement => new CsvDataSetWriter(this, fileStatement);
}
