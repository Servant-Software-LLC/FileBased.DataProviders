using Data.Common.FileStatements;
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

    private void JsonWatcher_Changed(object sender, FileSystemEventArgs e)
    {
        //we dont need to update anything if dataset is null
        if (DataSet == null)
        {
            return;
        }

        tablesToUpdate.Add(Path.GetFileNameWithoutExtension(e.FullPath));
    }

    public DataTable ReadFile(FileStatement fileStatement, TransactionScopedRows transactionScopedRows, bool shouldLock = false)
    {
        DataTable returnValue;

        if (shouldLock)
            FileWriter._rwLock.EnterReadLock();

        try
        {
            EnsureFileSystemWatcher();

            if (fileConnection.FolderAsDatabase)
            {
                DataSet ??= new DataSet();
                var newTables = fileStatement.Tables.Select(table => table.TableName).ToHashSet();

                ReadFromFolder(newTables.Where(x => DataSet.Tables[x] == null));
            }
            else if (DataSet == null)
            {
                ReadFromFile();
            }

            //Reload any of the tables from disk?
            CheckForTableReload();

            returnValue = GetResultset(fileStatement, transactionScopedRows);
        }
        finally
        {
            if (shouldLock)
                FileWriter._rwLock.ExitReadLock();
        }

        return returnValue;
    }

    IEnumerable<DataColumn> ITableSchemaProvider.GetColumns(SqlTable table)
    {
        var dataTable = DataSet.Tables[table.TableName];
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
            fileSelect.SqlSelect.ResolveReferences(databaseConnectionProvider, this, null);

            List<DataSet> dataSets = new();

            //Get a DataSet of all the tables in this query (minus the INFORMATION_SCHEMA tables requested)
            TransactionLevelData transactionLevelData = new(DataSet, fileConnection.Database, transactionScopedRows);
            var databaseData = transactionLevelData.Compose(fileStatement.Tables);
            dataSets.Add(databaseData);

            //Determine if we have any INFORMATION_SCHEMA tables
            (bool includeTableSchema, bool includeColumnSchema) = NeedsSchemaMetadata(fileStatement.Tables);
            if (includeTableSchema || includeColumnSchema)
            {
                DataSet metadataDataSet = new(SchemaDatabase);
                if (includeTableSchema)
                    metadataDataSet.Tables.Add(GenerateInformationSchemaTable());

                if (includeColumnSchema)
                    metadataDataSet.Tables.Add(GenerateInformationSchemaColumn());

                dataSets.Add(metadataDataSet);
            }

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

    private DataTable GenerateInformationSchemaTable()
    {
        var informationSchemaTable = new DataTable(SchemaTable);

        // Add columns to the DataTable
        informationSchemaTable.Columns.Add("TABLE_CATALOG", typeof(string));
        informationSchemaTable.Columns.Add("TABLE_NAME", typeof(string));
        informationSchemaTable.Columns.Add("TABLE_TYPE", typeof(string));

        IEnumerable<string> allTableNames;
        if (fileConnection.FolderAsDatabase)
        {
            //For FolderAsDatabase, we didn't want to force the costly operation of loading 
            //all files in the directory, when just trying to get a list of the tables.
            allTableNames = GetTableNamesFromFolderAsDatabase();
        }
        else
        {
            // If FileAsDatabase, then we've already loaded all the tables into the DataSet
            allTableNames = DataSet.Tables.Cast<DataTable>().Select(table => table.TableName);
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

    private (bool IncludeTableSchema, bool IncludeColumnSchema) NeedsSchemaMetadata(IEnumerable<SqlTable> tablesInvolvedInSqlStatement)
    {
        bool includeTableSchema = false;
        bool includeColumnSchema = false;

        foreach (SqlTable table in tablesInvolvedInSqlStatement)
        {
            //If the table requested is not in the INFORMATION_SCHEMA database
            if (string.Compare(table.DatabaseName, SchemaDatabase) != 0)
                continue;

            includeTableSchema = includeTableSchema || string.Compare(table.TableName, SchemaTable) == 0;
            includeColumnSchema = includeColumnSchema || string.Compare(table.TableName, SchemaColumn) == 0;
        }

        return (includeTableSchema, includeColumnSchema);
    }

    /// <summary>
    /// Read in files from a folder and creates DataTable instances for each name that matches <see cref="tableNames"/>
    /// </summary>
    /// <param name="tableNames"></param>
    protected abstract void ReadFromFolder(IEnumerable<string> tableNames);
    protected abstract void UpdateFromFolder(string tableName);
    
    /// <summary>
    /// Reads in file from disk, creates DataTable instances for every table and adds them to the <see cref="DataSet"/>
    /// </summary>
    protected abstract void ReadFromFile();
    protected abstract void UpdateFromFile();
   
    public void Dispose()
    {
        //_fileWatcher.Dispose();
        //DataSet.Dispose();
    }
}
