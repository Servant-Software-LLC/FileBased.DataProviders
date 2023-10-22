using Microsoft.Extensions.Logging;
using SqlBuildingBlocks.Interfaces;
using SqlBuildingBlocks.LogicalEntities;

namespace System.Data.FileClient;


/// <summary>
/// Represents a base class for file-based data adapters.
/// </summary>
/// <typeparam name="TFileParameter">The type of the file parameter.</typeparam>
public abstract class FileDataAdapter<TFileParameter> : IDataAdapter, IDisposable
    where TFileParameter : FileParameter<TFileParameter>, new()
{
    private ILogger<FileDataAdapter<TFileParameter>> log => connection.LoggerServices.CreateLogger<FileDataAdapter<TFileParameter>>();

    private DataRow? lastDataRowChanged;
    private FileConnection<TFileParameter>? connection;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileDataAdapter{TFileParameter}"/> class.
    /// </summary>
    public FileDataAdapter()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileDataAdapter{TFileParameter}"/> class.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="connection">The connection.</param>
    /// <exception cref="ArgumentException">
    /// '{nameof(query)}' cannot be null or empty.
    /// or
    /// connection
    /// </exception>
    public FileDataAdapter(string query, FileConnection<TFileParameter> connection)
    {
        if (string.IsNullOrEmpty(query))
        {
            throw new ArgumentException($"'{nameof(query)}' cannot be null or empty.", nameof(query));
        }
        this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
        SelectCommand = connection.CreateCommand();
        SelectCommand.Connection = connection;
        SelectCommand.CommandText = query;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileDataAdapter{TFileParameter}"/> class.
    /// </summary>
    /// <param name="selectCommand">The select command.</param>
    /// <exception cref="ArgumentNullException">selectCommand</exception>
    /// <exception cref="ArgumentException">
    /// connection
    /// or
    /// {nameof(SelectCommand)}.{nameof(SelectCommand.Connection)} is not a {nameof(FileConnection{TFileParameter})}
    /// or
    /// The {GetType()} cannot be used with an admin connection.
    /// or
    /// '{nameof(FileCommand{TFileParameter>.CommandText)}' cannot be null or empty.
    /// </exception>
    public FileDataAdapter(FileCommand<TFileParameter> selectCommand)
    {
        SelectCommand = selectCommand ?? throw new ArgumentNullException(nameof(selectCommand));

        if (SelectCommand.Connection == null)
            throw new ArgumentNullException(nameof(connection));

        if (SelectCommand.Connection is not FileConnection<TFileParameter> selectConnection)
            throw new ArgumentException($"{nameof(SelectCommand)}.{nameof(SelectCommand.Connection)} is not a {nameof(FileConnection<TFileParameter>)}");

        if (selectConnection.AdminMode)
            throw new ArgumentException($"The {GetType()} cannot be used with an admin connection.");

        connection = selectConnection;

        if (string.IsNullOrEmpty(SelectCommand.CommandText))
        {
            throw new ArgumentException($"'{nameof(FileCommand<TFileParameter>.CommandText)}' cannot be null or empty.", nameof(SelectCommand.CommandText));
        }
    }

    /// <summary>
    /// Gets or sets the select command.
    /// </summary>
    /// <value>
    /// The select command.
    /// </value>
    public IDbCommand? SelectCommand { get; set; }

    /// <summary>
    /// Gets or sets the update command.
    /// </summary>
    /// <value>
    /// The update command.
    /// </value>
    public IDbCommand? UpdateCommand { get; set; }

    /// <summary>
    /// Gets or sets the missing mapping action.
    /// </summary>
    /// <value>
    /// The missing mapping action.
    /// </value>
    public MissingMappingAction MissingMappingAction { get; set; }

    /// <summary>
    /// Gets or sets the missing schema action.
    /// </summary>
    /// <value>
    /// The missing schema action.
    /// </value>
    public MissingSchemaAction MissingSchemaAction { get; set; }

    /// <summary>
    /// Gets the table mappings.
    /// </summary>
    /// <value>
    /// The table mappings.
    /// </value>
    public ITableMappingCollection TableMappings { get; }

    /// <summary>
    /// Fills the specified data set.
    /// </summary>
    /// <param name="dataSet">The data set.</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">
    /// A connection cannot be inferred when calling the {nameof(Fill)} method
    /// or
    /// The {GetType()} cannot be used with an admin connection.
    /// or
    /// {nameof(SelectCommand)} is not set.
    /// or
    /// {nameof(SelectCommand.Connection)} property on {nameof(SelectCommand)} is not set.
    /// or
    /// {nameof(SelectCommand.CommandText)} property on {nameof(SelectCommand)} is not set.
    /// </exception>
    public int Fill(DataSet dataSet)
    {
        if (connection == null)
            throw new InvalidOperationException($"A connection cannot be inferred when calling the {nameof(Fill)} method");

        if (connection.AdminMode)
            throw new ArgumentException($"The {GetType()} cannot be used with an admin connection.");

        if (SelectCommand == null)
            throw new InvalidOperationException($"{nameof(SelectCommand)} is not set.");

        if (SelectCommand.Connection == null)
            throw new InvalidOperationException($"{nameof(SelectCommand.Connection)} property on {nameof(SelectCommand)} is not set.");

        if (string.IsNullOrEmpty(SelectCommand.CommandText))
            throw new InvalidOperationException($"{nameof(SelectCommand.CommandText)} property on {nameof(SelectCommand)} is not set.");

        log.LogInformation($"{GetType()}.{nameof(Fill)}() called.  SelectCommand.CommandText = {SelectCommand.CommandText}");

        if (SelectCommand is not FileCommand<TFileParameter> fileCommand)
            throw new InvalidOperationException($"{SelectCommand.GetType()} is not a FileCommand<> type.");

        var selectQuery = FileStatementCreator.CreateSelect(fileCommand, log);
        var fileReader = connection.FileReader;

        var transactionScopedRows = fileCommand.FileTransaction == null ? null : fileCommand.FileTransaction.TransactionScopedRows;
        var dataTable = fileReader.ReadFile(selectQuery, transactionScopedRows, true);

        var cols = GetColumns(dataTable, selectQuery);
        dataTable.Columns
            .Cast<DataColumn>()
            .Where(column => !cols.Contains(column.ColumnName))
            .Select(column => column.ColumnName)
            .ToList()
            .ForEach(dataTable.Columns.Remove);

        dataTable.RowChanged += DataTable_RowChanged;

        dataTable.TableName = "Table";
        dataSet.Tables.Clear();
        dataSet.Tables.Add(dataTable);
        return dataTable.Rows.Count;
    }

    /// <summary>
    /// Fills the schema.
    /// </summary>
    /// <param name="dataSet">The data set.</param>
    /// <param name="schemaType">Type of the schema.</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">
    /// {nameof(SelectCommand)} is not set.
    /// or
    /// {nameof(SelectCommand.Connection)} property on {nameof(SelectCommand)} is not set.
    /// or
    /// {nameof(SelectCommand.CommandText)} property on {nameof(SelectCommand)} is not set.
    /// </exception>
    public DataTable[] FillSchema(DataSet dataSet, SchemaType schemaType)
    {
        if (SelectCommand == null)
            throw new InvalidOperationException($"{nameof(SelectCommand)} is not set.");

        if (SelectCommand.Connection == null)
            throw new InvalidOperationException($"{nameof(SelectCommand.Connection)} property on {nameof(SelectCommand)} is not set.");

        if (string.IsNullOrEmpty(SelectCommand.CommandText))
            throw new InvalidOperationException($"{nameof(SelectCommand.CommandText)} property on {nameof(SelectCommand)} is not set.");

        connection = (FileConnection<TFileParameter>)SelectCommand.Connection;

        if (connection.AdminMode)
            throw new ArgumentException($"The {GetType()} cannot be used with an admin connection.");

        if (SelectCommand is not FileCommand<TFileParameter> fileCommand)
            throw new InvalidOperationException($"{SelectCommand.GetType()} is not a FileCommand<> type.");

        log.LogInformation($"{GetType()}.{nameof(FillSchema)}() called.  SelectCommand.CommandText = {SelectCommand.CommandText}");

        var selectQuery = FileStatementCreator.CreateSelect((FileCommand<TFileParameter>)SelectCommand, log);
        var fileReader = connection.FileReader;

        var transactionScopedRows = fileCommand.FileTransaction == null ? null : fileCommand.FileTransaction.TransactionScopedRows;
        var dataTable = fileReader.ReadFile(selectQuery, transactionScopedRows, true);
        var cols = GetColumns(dataTable, selectQuery);
        dataTable.Columns
            .Cast<DataColumn>()
            .Where(column => !cols.Contains(column.ColumnName))
            .Select(column => column.ColumnName)
            .ToList()
            .ForEach(dataTable.Columns.Remove);

        if (dataSet.Tables["Table"] != null)
        {
            dataSet.Tables.Remove("Table");
        }

        dataTable.Rows.Clear();
        return new DataTable[] { dataTable };
    }

    /// <summary>
    /// Gets the fill parameters.
    /// </summary>
    /// <returns></returns>
    public IDataParameter[] GetFillParameters()
    {
        IDataParameter[]? value = null;
        if (SelectCommand != null)
        {
            IDataParameterCollection parameters = SelectCommand.Parameters;
            if (parameters != null)
            {
                value = new IDataParameter[parameters.Count];
                parameters.CopyTo(value, 0);
            }
        }
        if (value == null)
        {
            value = Array.Empty<IDataParameter>();
        }
        return value;
    }

    /// <summary>
    /// Updates the specified data set.
    /// </summary>
    /// <param name="dataSet">The data set.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">dataSet cannot be null</exception>
    /// <exception cref="InvalidOperationException">
    /// Update requires a valid UpdateCommand when passed DataRow collection with modified rows.
    /// or
    /// Update requires the UpdateCommand to have a connection object. The Connection property of the UpdateCommand has not been initialized.
    /// or
    /// {nameof(UpdateCommand.CommandText)} property on {nameof(UpdateCommand)} is not set.
    /// </exception>
    public int Update(DataSet dataSet)
    {
        // dataSet.Tables.Clear();
        if (dataSet == null)
            throw new ArgumentNullException($"{nameof(dataSet)} cannot be null");

        if (UpdateCommand == null)
            throw new InvalidOperationException($"Update requires a valid UpdateCommand when passed DataRow collection with modified rows.");

        if (UpdateCommand.Connection == null)
            throw new InvalidOperationException($"Update requires the UpdateCommand to have a connection object. The Connection property of the UpdateCommand has not been initialized.");

        if (string.IsNullOrEmpty(UpdateCommand.CommandText))
            throw new InvalidOperationException($"{nameof(UpdateCommand.CommandText)} property on {nameof(UpdateCommand)} is not set.");

        connection = (FileConnection<TFileParameter>)UpdateCommand!.Connection!;

        if (connection.AdminMode)
            throw new ArgumentException($"The {GetType()} cannot be used with an admin connection.");

        log.LogInformation($"{GetType()}.{nameof(Update)}() called.  UpdateCommand.CommandText = {UpdateCommand.CommandText}");

        var query = FileStatementCreator.Create((FileCommand<TFileParameter>)UpdateCommand, log);
        if (query is not FileUpdate updateStatement)
        {
            throw new QueryNotSupportedException("This query is not yet supported via DataAdapter");
        }

        //check if source column is set and has parameters
        if (UpdateCommand.Parameters.Count > 0)
        {
            foreach (IDbDataParameter parameter in UpdateCommand.Parameters)
            {
                if (!string.IsNullOrEmpty(parameter.SourceColumn)
                    &&
                    lastDataRowChanged != null
                    &&
                    lastDataRowChanged.Table.Columns.Contains(parameter.SourceColumn))
                {
                    parameter.Value = lastDataRowChanged[parameter.SourceColumn];
                }
            }
        }

        var updater = CreateWriter(query);
        return updater.Execute();
    }

    /// <summary>
    /// Creates the writer.
    /// </summary>
    /// <param name="fileQuery">The file query.</param>
    /// <returns></returns>
    protected abstract FileWriter CreateWriter(FileStatement fileQuery);

    private void DataTable_RowChanged(object sender, DataRowChangeEventArgs e)
    {
        if (e.Action == DataRowAction.Change)
        {
            lastDataRowChanged = e.Row;
        }
    }

    private IEnumerable<string> GetColumns(DataTable dataTable, FileSelect selectQuery)
    {
        List<string> columnNames = new();
        foreach (ISqlColumn iSqlColumn in selectQuery.Columns)
        {
            switch (iSqlColumn)
            {
                case SqlAllColumns sqlAllColumns:
                    columnNames.Clear();
                    foreach (DataColumn dataColumn in dataTable.Columns)
                    {
                        columnNames.Add(dataColumn.ColumnName);
                    }
                    return columnNames;

                case SqlColumn sqlColumn:
                    columnNames.Add(sqlColumn.ColumnName);
                    break;

                default:
                    throw new Exception($"Column was of an unresolved type that was unexpected: {iSqlColumn}({iSqlColumn.GetType()})");
            }
        }

        return columnNames;
    }

    /// <summary>
    /// Disposes this instance.
    /// </summary>
    public void Dispose()
    {
    }
}
