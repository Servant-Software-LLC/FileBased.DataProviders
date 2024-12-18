using Microsoft.Extensions.Logging;
using SqlBuildingBlocks.Interfaces;
using SqlBuildingBlocks.LogicalEntities;

namespace System.Data.FileClient;


/// <summary>
/// Represents a base class for file-based data adapters.
/// </summary>
/// <typeparam name="TFileParameter">The type of the file parameter.</typeparam>
public abstract class FileDataAdapter<TFileParameter> : DbDataAdapter, IDisposable
    where TFileParameter : FileParameter<TFileParameter>, new()
{
    private ILogger<FileDataAdapter<TFileParameter>> log => Connection.LoggerServices.CreateLogger<FileDataAdapter<TFileParameter>>();

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

        SelectCommand = connection.CreateCommand();
        SelectCommand.Connection = connection ?? throw new ArgumentNullException(nameof(connection));
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
            throw new ArgumentNullException($"{nameof(SelectCommand)}.{nameof(SelectCommand.Connection)}");

        if (SelectCommand.Connection is not FileConnection<TFileParameter> selectConnection)
            throw new ArgumentException($"{nameof(SelectCommand)}.{nameof(SelectCommand.Connection)} is not a {nameof(FileConnection<TFileParameter>)}");

        if (selectConnection.AdminMode)
            throw new ArgumentException($"The {GetType()} cannot be used with an admin connection.");

        if (string.IsNullOrEmpty(SelectCommand.CommandText))
        {
            throw new ArgumentException($"'{nameof(FileCommand<TFileParameter>.CommandText)}' cannot be null or empty.", nameof(SelectCommand.CommandText));
        }
    }

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
    public override int Fill(DataSet dataSet)
    {
        if (SelectCommand == null)
            throw new InvalidOperationException($"{nameof(SelectCommand)} is not set.");

        if (SelectCommand.Connection == null)
            throw new InvalidOperationException($"{nameof(SelectCommand.Connection)} property on {nameof(SelectCommand)} is not set.");

        if (Connection.AdminMode)
            throw new ArgumentException($"The {GetType()} cannot be used with an admin connection.");

        if (string.IsNullOrEmpty(SelectCommand.CommandText))
            throw new InvalidOperationException($"{nameof(SelectCommand.CommandText)} property on {nameof(SelectCommand)} is not set.");

        return AutoOpenCloseConnection(dataSet, Fill_Inner);
    }

    public new int Fill(DataTable dataTable)
    {
        //NOTE:  Performance could be improved here especially with large sets of data.
        //       The SqlBuildingBlocks.QueryEngine returns a DataTable for its results, but if instead (or how another overload)
        //       which took a DataTable instance, then we could avoid all the copying of data seen in this method.
        var resultingTable = AutoOpenCloseConnection(ExecuteSelectCommand);

        //Copy the resultingTable into the provided dataTable
        dataTable.Clear();
        dataTable.Columns.Clear();

        // Copy the schema from the source table
        foreach (DataColumn column in resultingTable.Columns)
        {
            dataTable.Columns.Add(column.ColumnName, column.DataType);
        }

        // Import rows from the source table
        foreach (DataRow row in resultingTable.Rows)
        {
            dataTable.ImportRow(row);
        }

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
    public override DataTable[] FillSchema(DataSet dataSet, SchemaType schemaType)
    {
        if (SelectCommand == null)
            throw new InvalidOperationException($"{nameof(SelectCommand)} is not set.");

        if (SelectCommand.Connection == null)
            throw new InvalidOperationException($"{nameof(SelectCommand.Connection)} property on {nameof(SelectCommand)} is not set.");

        if (string.IsNullOrEmpty(SelectCommand.CommandText))
            throw new InvalidOperationException($"{nameof(SelectCommand.CommandText)} property on {nameof(SelectCommand)} is not set.");

        if (Connection.AdminMode)
            throw new ArgumentException($"The {GetType()} cannot be used with an admin connection.");

        return AutoOpenCloseConnection(dataSet, FillSchema_Inner);
    }

    /// <summary>
    /// Gets the fill parameters.
    /// </summary>
    /// <returns></returns>
    public override IDataParameter[] GetFillParameters()
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
    public override int Update(DataSet dataSet)
    {
        if (dataSet == null)
            throw new ArgumentNullException($"{nameof(dataSet)} cannot be null");

        if (dataSet.Tables.Count == 0)
            throw new InvalidOperationException($"{nameof(dataSet)} does not contain any tables.");

        if (UpdateCommand == null)
            throw new InvalidOperationException($"Update requires a valid UpdateCommand when passed DataRow collection with modified rows.");

        if (UpdateCommand.Connection == null)
            throw new InvalidOperationException($"Update requires the UpdateCommand to have a connection object. The Connection property of the UpdateCommand has not been initialized.");

        if (string.IsNullOrEmpty(UpdateCommand.CommandText))
            throw new InvalidOperationException($"{nameof(UpdateCommand.CommandText)} property on {nameof(UpdateCommand)} is not set.");

        if (Connection.AdminMode)
            throw new ArgumentException($"The {GetType()} cannot be used with an admin connection.");

        log.LogInformation($"{GetType()}.{nameof(Update)}() called.  UpdateCommand.CommandText = {UpdateCommand.CommandText}");

        var dataTable = dataSet.Tables[0];
        var changedDataRows = dataTable.Rows.Cast<DataRow>().Where(row => row.RowState == DataRowState.Modified).ToList();
        int rowsAffected = 0;
        foreach (DataRow changedDataRow in changedDataRows)
        {
            //check if source column is set and has parameters
            if (UpdateCommand.Parameters.Count > 0)
            {
                foreach (IDbDataParameter parameter in UpdateCommand.Parameters)
                {
                    if (!string.IsNullOrEmpty(parameter.SourceColumn))
                    {
                        if (!dataTable.Columns.Contains(parameter.SourceColumn))
                            throw new InvalidOperationException($"The source column '{parameter.SourceColumn}' does not exist in the DataTable passed into the Update method.");

                        parameter.Value = changedDataRow[parameter.SourceColumn];
                    }
                }
            }

            var query = FileStatementCreator.Create((FileCommand<TFileParameter>)UpdateCommand, log);
            if (query is not FileUpdate updateStatement)
            {
                throw new QueryNotSupportedException("This query is not yet supported via DataAdapter");
            }

            var updater = CreateWriter(query);
            rowsAffected += updater.Execute();
        }

        return rowsAffected;
    }

    /// <summary>
    /// Creates the writer.
    /// </summary>
    /// <param name="fileQuery">The file query.</param>
    /// <returns></returns>
    protected abstract FileWriter CreateWriter(FileStatement fileQuery);

    private FileConnection<TFileParameter> Connection
    {
        get
        {
            if (SelectCommand == null || SelectCommand.Connection == null)
                return null;

            if (SelectCommand.Connection is not FileConnection<TFileParameter> fileConnection)
            {
                throw new InvalidOperationException($"The SelectCommand.Connection is not a {typeof(FileConnection<TFileParameter>)}.  Type={SelectCommand.Connection.GetType()}");
            }

            return fileConnection;
        }
    }

    private DataTable[] FillSchema_Inner(DataSet dataSet)
    {
        if (SelectCommand is not FileCommand<TFileParameter> fileCommand)
            throw new InvalidOperationException($"{SelectCommand.GetType()} is not a FileCommand<> type.");

        log.LogInformation($"{GetType()}.{nameof(FillSchema)}() called.  SelectCommand.CommandText = {SelectCommand.CommandText}");

        var selectQuery = FileStatementCreator.CreateSelect((FileCommand<TFileParameter>)SelectCommand, log);
        var fileReader = Connection.FileReader;

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

    private int Fill_Inner(DataSet dataSet)
    {
        log.LogInformation($"{GetType()}.{nameof(Fill)}() called.  SelectCommand.CommandText = {SelectCommand.CommandText}");

        DataTable dataTable = ExecuteSelectCommand();

        dataTable.TableName = "Table";
        dataSet.Tables.Clear();
        dataSet.Tables.Add(dataTable);
        return dataTable.Rows.Count;
    }

    private DataTable ExecuteSelectCommand()
    {
        if (SelectCommand is not FileCommand<TFileParameter> fileCommand)
            throw new InvalidOperationException($"{SelectCommand.GetType()} is not a FileCommand<> type.");

        var selectQuery = FileStatementCreator.CreateSelect(fileCommand, log);
        var fileReader = Connection.FileReader;

        var transactionScopedRows = fileCommand.FileTransaction == null ? null : fileCommand.FileTransaction.TransactionScopedRows;
        var dataTable = fileReader.ReadFile(selectQuery, transactionScopedRows, true);

        var cols = GetColumns(dataTable, selectQuery);
        dataTable.Columns
            .Cast<DataColumn>()
            .Where(column => !cols.Contains(column.ColumnName))
            .Select(column => column.ColumnName)
            .ToList()
            .ForEach(dataTable.Columns.Remove);
        return dataTable;
    }

    private void AutoOpenCloseConnection(Action action)
    {
        var connection = Connection;

        //Determine if the connection has been opened.  Typical behavior is that the connection will be automatically opened if it is closed and if so, then will be closed at the end of the Fill operation.
        bool connectionOpened = connection.State == ConnectionState.Open;
        bool overrideCreateIfNotExists = !connection.CreateIfNotExist;
        if (!connectionOpened)
        {
            if (overrideCreateIfNotExists)
            {
                connection.CreateIfNotExist = true;
            }

            connection.Open();
        }

        try
        {
            action();
        }
        finally
        {
            if (!connectionOpened)
            {
                if (overrideCreateIfNotExists)
                {
                    connection.CreateIfNotExist = false;
                }

                connection.Close();
            }
        }
    }

    private TReturn AutoOpenCloseConnection<TReturn>(Func<TReturn> func)
    {
        TReturn returnValue = default;
        Action innerAction = () => returnValue = func();
        AutoOpenCloseConnection(innerAction);
        return returnValue;
    }

    private TReturn AutoOpenCloseConnection<TReturn>(DataSet dataSet, Func<DataSet, TReturn> func)
    {
        TReturn returnValue = default;
        Action innerAction = () => returnValue = func(dataSet);
        AutoOpenCloseConnection(innerAction);
        return returnValue;
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

}
