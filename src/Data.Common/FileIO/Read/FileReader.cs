using Data.Common.Utils;
using SqlBuildingBlocks.Interfaces;
using SqlBuildingBlocks.LogicalEntities;
using SqlBuildingBlocks.QueryProcessing;

namespace Data.Common.FileIO.Read;

public abstract class FileReader : ITableSchemaProvider, IDisposable
{
    private const string SchemaDatabase = "INFORMATION_SCHEMA";
    private const string SchemaTable = "TABLES";
    private const string SchemaColumn = "COLUMNS";

    private FileSystemWatcher? fileWatcher;
    protected readonly IFileConnection fileConnection;
    private readonly HashSet<string> tablesToUpdate = new();

    public DataSet? DataSet { get; protected set; }
    public DataSet? SchemaDataSet { get; protected set; }

    /// <summary>
    /// Read in file (representing a table) from a folder and creates DataTable instance for the matching <see cref="tableName"/>
    /// </summary>
    /// <param name="tableName">Name of the table file to read from folder</param>
    /// 
    protected abstract void ReadFromFolder(string tableName);
    protected abstract void UpdateFromFolder(string tableName);
    protected virtual bool ExistsInFolder(string tableName)
    {
        var path = fileConnection.GetTablePath(tableName);
        return File.Exists(path);
    }

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

    public void StartWatching()
    {
        if (fileWatcher != null)
        {

            fileWatcher.Changed -= JsonWatcher_Changed;
            fileWatcher.Changed += JsonWatcher_Changed;
        }
    }
    public void StopWatching()
    {
        if (fileWatcher != null)
            fileWatcher.Changed -= JsonWatcher_Changed;
    }

    public DataTable ReadFile(FileStatement fileStatement, TransactionScopedRows transactionScopedRows, bool shouldLock = false)
    {
        DataTable returnValue;

        if (shouldLock)
            FileWriter._rwLock.EnterReadLock();

        try
        {
            EnsureFileSystemWatcher();

            //Determine if we have any INFORMATION_SCHEMA tables
            (bool includeTableSchema, bool includeColumnSchema) = NeedsSchemaMetadata(fileStatement.Tables);

            if (fileConnection.FolderAsDatabase)
            {
                DataSet ??= new DataSet();

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

                foreach (var table in newTables.Where(x => DataSet.Tables[x] == null))
                {
                    try
                    {
                        ReadFromFolder(table);
                    }        
                    catch (Exception ex)
                    {
                        throw new TableNotFoundException($"Table '{table}' not found as file, {fileConnection.GetTableFileName(table)}, in '{fileConnection.Database}'", ex);
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
                FileWriter._rwLock.ExitReadLock();
        }

        return returnValue;
    }

    public bool TableExists(string tableName)
    {
        FileWriter._rwLock.EnterReadLock();

        try
        {
            EnsureFileSystemWatcher();

            if (fileConnection.FolderAsDatabase)
            {
                DataSet ??= new DataSet();

                return ExistsInFolder(tableName);
            }

            if (DataSet == null)
            {
                ReadFromFile();

            }
            return DataSet.Tables.Contains(tableName);
        }
        finally
        {
            FileWriter._rwLock.ExitReadLock();
        }
    }


    private void JsonWatcher_Changed(object sender, FileSystemEventArgs e)
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
        var dataSet = isSchemaTable ? SchemaDataSet : DataSet;
        var dataTable = dataSet.Tables[table.TableName];
        return dataTable.Columns.Cast<DataColumn>();
    }

    private void CheckForTableReload()
    {
        if (tablesToUpdate.Count > 0)
        {
            if (fileConnection.PathType == PathType.File)
            {
                UpdateFromFile();
            }
            else
            {
                for (int i = 0; i < tablesToUpdate.Count; i++)
                {
                    string? tableName = tablesToUpdate.ElementAt(i);
                    UpdateFromFolder(tableName);
                }
            }
            tablesToUpdate.Clear();
        }

    }

    private DataTable GetResultset(FileStatement fileStatement, TransactionScopedRows transactionScopedRows)
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

            List<DataSet> dataSets = new();

            //Get a DataSet of all the tables in this query (minus the INFORMATION_SCHEMA tables requested)
            TransactionLevelData transactionLevelData = new(DataSet, databaseConnectionProvider.DefaultDatabase, transactionScopedRows);
            var databaseData = transactionLevelData.Compose(fileStatement.Tables);
            dataSets.Add(databaseData);

            if (SchemaDataSet != null)
                dataSets.Add(SchemaDataSet);

            QueryEngine queryEngine = new(dataSets, fileSelect.SqlSelect);
            return queryEngine.QueryAsDataTable();
        }

        //Parser is not a FileSelect
        //In this case, the FileWriter that made this call, needs the actual DataTable so that it can use a DataView to
        //preform the write type of operations (DELETE, UPDATE, INSERT)
        return GetDataTable(fileStatement!.FromTable.TableName, transactionScopedRows)!;
    }


    private DataTable GetDataTable(string tableName, TransactionScopedRows transactionScopedRows)
    {
        var table = DataSet!.Tables[tableName]!;
        if (table == null)
            throw new TableNotFoundException($"Table '{tableName}' not found in '{fileConnection.Database}'");

        //TODO: Not clear if we need to add the transaction scoped rows or not.  Leaving for now, to follow same behavior as it was before 
        //      SqlBuildingBlocks and the QueryEngine were introduced.
        if (transactionScopedRows != null && transactionScopedRows.TryGetValue(tableName, out List<DataRow> additionalRows))
        {
            table = table.Copy();

            foreach (DataRow additionalRow in additionalRows)
            {
                var newRow = table.NewRow();

                // Copy the data.
                for (int i = 0; i < additionalRow.Table.Columns.Count; i++)
                {
                    newRow[i] = additionalRow[i];
                }

                table.Rows.Add(newRow);
            }
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
            allTableNames = DataSet.Tables.Cast<DataTable>().Select(table => table.TableName);
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
        var allTableColumns = DataSet.Tables.Cast<DataTable>()
            .SelectMany(table => table.Columns.Cast<DataColumn>().Select(column => (Table: table, Column: column)))
            .OrderBy(tuple => tuple.Table.TableName).ThenBy(tuple => tuple.Column.ColumnName).AsEnumerable();

        //TODO: When we upgrade to C# 12, use an alias for tuple type with the using directive.  REF:  https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/value-tuples
        foreach ((DataTable Table, DataColumn Column) columnTuple in allTableColumns)
        {
            AddRowToSchemaColumn(informationSchemaColumn, columnTuple.Table, columnTuple.Column);
        }

        return informationSchemaColumn;
    }

    private void AddRowToSchemaColumn(DataTable schemaTable, DataTable table, DataColumn column)
    {
        var row = schemaTable.NewRow();
        row["TABLE_CATALOG"] = Path.GetFileNameWithoutExtension(fileConnection.Database);
        row["TABLE_NAME"] = table.TableName;
        row["COLUMN_NAME"] = column.ColumnName;
        row["DATA_TYPE"] = column.DataType.FullName;
        schemaTable.Rows.Add(row);
    }


    private void EnsureFileSystemWatcher()
    {
        if (fileWatcher == null)
        {
            fileWatcher = new FileSystemWatcher();
            fileWatcher.NotifyFilter = NotifyFilters.LastWrite;
            if (fileConnection.FolderAsDatabase)
            {
                fileWatcher.Path = fileConnection.Database;
                fileWatcher.Filter = $"*.{fileConnection.FileExtension}";
            }
            else
            {
                var file = new FileInfo(fileConnection.Database);
                fileWatcher.Path = file.DirectoryName!;
                fileWatcher.Filter = file.Name;
            }
            fileWatcher.EnableRaisingEvents = true;
        }
    }
  

    private IEnumerable<string> GetFilesFromFolderAsDatabase() => fileConnection.FolderAsDatabase ?
        Directory.GetFiles(fileConnection.Database, $"*.{fileConnection.FileExtension}") :
        throw new ArgumentException($"The file connection for {GetType()} doesn't have a DataSource which is a folder.");

    private IEnumerable<string> GetTableNamesFromFolderAsDatabase() => GetFilesFromFolderAsDatabase().Select(x => Path.GetFileNameWithoutExtension(x));

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
