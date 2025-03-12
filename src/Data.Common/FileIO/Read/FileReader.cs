using Data.Common.DataSource;
using Data.Common.Utils;
using SqlBuildingBlocks.Interfaces;
using SqlBuildingBlocks.LogicalEntities;
using SqlBuildingBlocks.POCOs;
using SqlBuildingBlocks.QueryProcessing;
using SqlBuildingBlocks.Utils;

namespace Data.Common.FileIO.Read;

public abstract class FileReader : ITableSchemaProvider, IDisposable
{
    private const string SchemaDatabase = "INFORMATION_SCHEMA";
    private const string SchemaTable = "TABLES";
    private const string SchemaColumn = "COLUMNS";

    protected readonly IFileConnection fileConnection;
    private readonly HashSet<string> tablesToUpdate = new();

    public VirtualDataSet DataSet { get; protected set; }
    public DataSet SchemaDataSet { get; protected set; }

    public void MarkTableToUpdate(string tableName) => tablesToUpdate.Add(tableName);
    public void FreeDataSet()
    {
        DataSet.Dispose();
        DataSet = null;
    }

    /// <summary>
    /// Read in file (representing a table) from a folder and creates DataTable instance for the matching <see cref="tableName"/>
    /// </summary>
    /// <param name="tableName">Name of the table file to read from folder</param>
    /// 
    protected abstract void ReadFromFolder(string tableName);
    protected abstract void UpdateFromFolder(string tableName);

    /// <summary>
    /// Reads in file from disk, creates DataTable instances for every table and adds them to the <see cref="DataSet"/>
    /// </summary>
    protected abstract void ReadFromFile();
    protected abstract void UpdateFromFile();

    public FileReader(IFileConnection fileConnection)
    {
        this.fileConnection = fileConnection ?? throw new ArgumentNullException(nameof(fileConnection));

        DataSet = null;
    }

    public void StartWatching() => fileConnection.DataSourceProvider.StartWatching();

    public void StopWatching() => fileConnection.DataSourceProvider.StopWatching();

    public VirtualDataTable ReadFile(FileStatement fileStatement, TransactionScopedRows transactionScopedRows, bool shouldLock = false)
    {
        VirtualDataTable returnValue;

        if (shouldLock)
            FileWriter.readerWriterLock.EnterReadLock();

        try
        {
            EnsureFileSystemWatcher();

            //Determine if we have any INFORMATION_SCHEMA tables
            (bool includeTableSchema, bool includeColumnSchema) = NeedsSchemaMetadata(fileStatement.Tables);

            if (fileConnection.FolderAsDatabase)
            {
                DataSet ??= new VirtualDataSet();

                //Get table names that aren't schema tables.
                HashSet<string> newTables;
                if (includeColumnSchema)
                {
                    //If the column schema table is requested, then we need to load all the tables.
                    newTables = GetTableNamesFromFolderAsDatabase().ToHashSet();
                }
                else
                {
                    //Load only the tables in the SQL statement.
                    newTables = fileStatement.Tables.Where(table => !IsSchemaTable(table)).Select(table => table.TableName).ToHashSet();
                }

                foreach (var table in newTables.Where(x => !DataSet.Tables.Contains(x)))
                {
                    try
                    {
                        ReadFromFolder(table);
                    }        
                    catch (Exception ex)
                    {
                        throw new TableNotFoundException($"Table '{table}' not found as file, {fileConnection.DataSourceProvider.StorageIdentifier(table)}, in '{fileConnection.Database}'", ex);
                    }
                }
            }
            else if (DataSet == null)
            {
                ReadFromFile();
            }

            //Reload any of the tables from disk?
            CheckForTableReload();

            UpdateSchemaDataSet(includeTableSchema, includeColumnSchema);

            returnValue = GetResultset(fileStatement, transactionScopedRows);
        }
        finally
        {
            if (shouldLock)
                FileWriter.readerWriterLock.ExitReadLock();
        }

        return returnValue;
    }

    public bool TableExists(string tableName)
    {
        FileWriter.readerWriterLock.EnterReadLock();

        try
        {
            EnsureFileSystemWatcher();

            if (fileConnection.FolderAsDatabase)
            {
                DataSet ??= new VirtualDataSet();

                //Optimization: Only need to check if the file exists in the folder.  We don't need to load the table into memory.
                return fileConnection.DataSourceProvider.StorageExists(tableName);
            }

            if (DataSet == null)
            {
                ReadFromFile();

            }
            return DataSet.Tables.Contains(tableName);
        }
        finally
        {
            FileWriter.readerWriterLock.ExitReadLock();
        }
    }

    public int? ColumnsOnTable(string tableName)
    {
        FileWriter.readerWriterLock.EnterReadLock();

        try
        {
            EnsureFileSystemWatcher();

            if (fileConnection.FolderAsDatabase)
            {
                DataSet ??= new VirtualDataSet();

                // Check if the file exists in the folder.  If it doesn't, then the table doesn't exist.
                if (!fileConnection.DataSourceProvider.StorageExists(tableName))
                    return null;

                if (DataSet.Tables.Contains(tableName))
                    DataSet.Tables.Remove(tableName);

                ReadFromFolder(tableName);
            } 
            else if (DataSet == null)
            {
                ReadFromFile();

            }

            if (!DataSet.Tables.Contains(tableName))
                return null;

            var columns = DataSet.Tables[tableName].Columns;
            if (columns == null)
                return 0;

            return columns.Count;
        }
        finally
        {
            FileWriter.readerWriterLock.ExitReadLock();
        }
    }

    private void FileWatcher_Changed(object sender, FileSystemEventArgs e)
    {
        //we dont need to update anything if dataset is null
        if (DataSet == null)
        {
            return;
        }

        tablesToUpdate.Add(Path.GetFileNameWithoutExtension(e.FullPath));
    }

    private void UpdateSchemaDataSet(bool includeTableSchema, bool includeColumnSchema)
    {
        if (includeTableSchema || includeColumnSchema)
        {
            DataSet metadataDataSet = new(SchemaDatabase);
            if (includeTableSchema)
                metadataDataSet.Tables.Add(GenerateInformationSchemaTable(includeColumnSchema));

            if (includeColumnSchema)
                metadataDataSet.Tables.Add(GenerateInformationSchemaColumn());

            SchemaDataSet = metadataDataSet;
        }
    }

    IEnumerable<DataColumn> ITableSchemaProvider.GetColumns(SqlTable table)
    {
        var isSchemaTable = string.Compare(table.DatabaseName, SchemaDatabase, true) == 0;

        // Special DataSet for schema tables.
        if (isSchemaTable)
        {
            var dataTable = SchemaDataSet.Tables[table.TableName];
            return dataTable.Columns.Cast<DataColumn>();
        }

        // DataSet table
        var virtualDataTable = DataSet.Tables[table.TableName];
        return virtualDataTable.Columns.Cast<DataColumn>();
    }

    private void CheckForTableReload()
    {
        if (tablesToUpdate.Count > 0)
        {
            if (fileConnection.DataSourceType == DataSourceType.File)
            {
                UpdateFromFile();
            }
            else
            {
                for (int i = 0; i < tablesToUpdate.Count; i++)
                {
                    string tableName = tablesToUpdate.ElementAt(i);
                    UpdateFromFolder(tableName);
                }
            }
            tablesToUpdate.Clear();
        }

    }

    private VirtualDataTable GetResultset(FileStatement fileStatement, TransactionScopedRows transactionScopedRows)
    {
        if (fileStatement is FileSelect fileSelect)
        {
            //Parser is FileSelect

            //We defered the resolution, so that we ensure that the schema of tables that we care about has already been loaded from disk
            //and we can just grab it from the DataSet property.
            DatabaseConnectionProvider databaseConnectionProvider = new(fileConnection);

            //IFunctionProvider instance is not provided here, because resolution of the functions require state from previous command executions.
            //This is accomplished calling SqlSelect.Accept() using ResolveBuiltinFunctionsVisitor instance as the parameter
            fileSelect.SqlSelect.ResolveReferences(databaseConnectionProvider, this, null);

            if (fileSelect.SqlSelect.InvalidReferences)
                throw new InvalidOperationException($"Unable to resolve the references with the SELECT statement.  Reason: {fileSelect.SqlSelect.InvalidReferenceReason}");

            List<ITableDataProvider> tableDataProviders = new();

            //Get a DataSet of all the tables in this query (minus the INFORMATION_SCHEMA tables requested)
            var databaseName = databaseConnectionProvider.DefaultDatabase;
            TransactionLevelData transactionLevelData = new(DataSet, databaseName, transactionScopedRows);
            var databaseData = transactionLevelData.Compose(fileStatement.Tables);
            var tableDataProviderAdaptor = new TableDataProviderAdaptor();
            tableDataProviderAdaptor.AddDataSet(databaseName, databaseData);
            tableDataProviders.Add(tableDataProviderAdaptor);

            if (SchemaDataSet != null)
            {
                var adaptor = new TableDataProviderAdaptor(new DataSet[] { SchemaDataSet });
                tableDataProviders.Add(adaptor);
            }

            AllTableDataProvider allTableDataProvider = new AllTableDataProvider(tableDataProviders);
            QueryEngine queryEngine = new(allTableDataProvider, fileSelect.SqlSelect);
            var virtualDataTable = queryEngine.Query();
            return virtualDataTable;
        }

        //Parser is not a FileSelect
        //In this case, the FileWriter that made this call, needs the actual DataTable so that it can use a DataView to
        //preform the write type of operations (DELETE, UPDATE, INSERT)
        var dataTable = GetDataTable(fileStatement!.FromTable.TableName, transactionScopedRows)!;
        return dataTable;
    }


    private VirtualDataTable GetDataTable(string tableName, TransactionScopedRows transactionScopedRows)
    {
        var table = DataSet!.Tables[tableName]!;
        if (table == null)
            throw new TableNotFoundException($"Table '{tableName}' not found in '{fileConnection.Database}'");

        //TODO: Not clear if we need to add the transaction scoped rows or not.  Leaving for now, to follow same behavior as it was before 
        //      SqlBuildingBlocks and the QueryEngine were introduced.
        if (transactionScopedRows != null && transactionScopedRows.TryGetValue(tableName, out List<DataRow> additionalRows))
        {
            DataTable clonedTable = new DataTable(tableName);

            //Copy the columns
            foreach (DataColumn column in table.Columns)
            {
                clonedTable.Columns.Add(new DataColumn(column.ColumnName, column.DataType));
            }

            //Copy the data rows
            foreach (DataRow row in table.Rows)
            {
                clonedTable.Rows.Add(row.ItemArray);
            }

            foreach (DataRow additionalRow in additionalRows)
            {
                clonedTable.Rows.Add(additionalRow.ItemArray);
            }

            table = new VirtualDataTable(clonedTable);
        }

        return table;
    }

    private DataTable GenerateInformationSchemaTable(bool includeColumnSchema)
    {
        var informationSchemaTable = new DataTable(SchemaTable);

        // Add columns to the DataTable
        informationSchemaTable.Columns.Add("TABLE_CATALOG", typeof(string));
        informationSchemaTable.Columns.Add("TABLE_NAME", typeof(string));
        informationSchemaTable.Columns.Add("TABLE_TYPE", typeof(string));

        IEnumerable<string> allTableNames;
        if (includeColumnSchema || !fileConnection.FolderAsDatabase)
        {
            // If the COLUMNS table is requested or FileAsDatabase, then we've already loaded all the tables into the DataSet
            allTableNames = DataSet.Tables.Cast<VirtualDataTable>().Select(table => table.TableName);
        }
        else
        {
            //For FolderAsDatabase, we didn't want to force the costly operation of loading 
            //all files in the directory, when just trying to get a list of the tables.
            allTableNames = GetTableNamesFromFolderAsDatabase();
        }

        foreach (string tableName in allTableNames)
        {
            AddRowToSchemaTable(informationSchemaTable, tableName, fileConnection.FolderAsDatabase);
        }

        return informationSchemaTable;
    }

    private void AddRowToSchemaTable(DataTable schemaTable, string tableName, bool folderAsDatabase)
    {
        var row = schemaTable.NewRow();
        row["TABLE_CATALOG"] = folderAsDatabase ? Path.GetFileName(fileConnection.Database) : Path.GetFileNameWithoutExtension(fileConnection.Database);
        row["TABLE_NAME"] = tableName;
        row["TABLE_TYPE"] = "BASE TABLE";
        schemaTable.Rows.Add(row);
    }


    private DataTable GenerateInformationSchemaColumn()
    {
        var informationSchemaColumn = new DataTable(SchemaColumn);

        // Add columns to the DataTable
        informationSchemaColumn.Columns.Add("TABLE_CATALOG", typeof(string));
        informationSchemaColumn.Columns.Add("TABLE_NAME", typeof(string));
        informationSchemaColumn.Columns.Add("COLUMN_NAME", typeof(string));
        informationSchemaColumn.Columns.Add("DATA_TYPE", typeof(string));

        //Get all table colums sorted in order of the primary key (database, table, column)
        var allTableColumns = DataSet.Tables.Cast<VirtualDataTable>()
            .SelectMany(table => table.Columns.Cast<DataColumn>().Select(column => (Table: table, Column: column)))
            .OrderBy(tuple => tuple.Table.TableName).ThenBy(tuple => tuple.Column.ColumnName).AsEnumerable();

        //TODO: When we upgrade to C# 12, use an alias for tuple type with the using directive.  REF:  https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/value-tuples
        foreach ((VirtualDataTable Table, DataColumn Column) columnTuple in allTableColumns)
        {
            AddRowToSchemaColumn(informationSchemaColumn, columnTuple.Table, columnTuple.Column);
        }

        return informationSchemaColumn;
    }

    private void AddRowToSchemaColumn(DataTable schemaTable, VirtualDataTable table, DataColumn column)
    {
        var row = schemaTable.NewRow();
        row["TABLE_CATALOG"] = Path.GetFileNameWithoutExtension(fileConnection.Database);
        row["TABLE_NAME"] = table.TableName;
        row["COLUMN_NAME"] = column.ColumnName;
        row["DATA_TYPE"] = column.DataType.FullName;
        schemaTable.Rows.Add(row);
    }


    private void EnsureFileSystemWatcher() => fileConnection.DataSourceProvider.EnsureWatcher();  

    private IEnumerable<string> GetTableNamesFromFolderAsDatabase() => fileConnection.FolderAsDatabase ?
        fileConnection.DataSourceProvider.GetTableNames() :
        throw new ArgumentException($"The file connection for {GetType()} doesn't have a DataSource which is a folder.");

    private static bool IsSchemaTable(SqlTable sqlTable) => string.Compare(sqlTable.DatabaseName, SchemaDatabase) == 0;

    private (bool IncludeTableSchema, bool IncludeColumnSchema) NeedsSchemaMetadata(IEnumerable<SqlTable> tablesInvolvedInSqlStatement)
    {
        bool includeTableSchema = false;
        bool includeColumnSchema = false;

        foreach (SqlTable table in tablesInvolvedInSqlStatement)
        {
            //If the table requested is not in the INFORMATION_SCHEMA database
            if (!IsSchemaTable(table))
                continue;

            includeTableSchema = includeTableSchema || string.Compare(table.TableName, SchemaTable) == 0;
            includeColumnSchema = includeColumnSchema || string.Compare(table.TableName, SchemaColumn) == 0;
        }

        return (includeTableSchema, includeColumnSchema);
    }
   
    public void Dispose()
    {
        //_fileWatcher.Dispose();
        //DataSet.Dispose();
    }
}
