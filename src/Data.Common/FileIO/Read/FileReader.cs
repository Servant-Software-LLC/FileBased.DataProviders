namespace Data.Common.FileIO.Read;

public abstract class FileReader<TFileParameter> : IDisposable
    where TFileParameter : FileParameter<TFileParameter>, new()
{
    private const string SchemaTable = "INFORMATION_SCHEMA.TABLES";
    private const string SchemaColumn = "INFORMATION_SCHEMA.COLUMNS";
    private FileSystemWatcher? fileWatcher;
    protected readonly FileConnection<TFileParameter> fileConnection;
    private readonly HashSet<string> tablesToUpdate = new();

    public DataSet? DataSet { get; protected set; }

    public FileReader(FileConnection<TFileParameter> fileConnection)
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

    public static bool IsSchemaTable(string tableName) =>
        string.Compare(tableName, SchemaTable, StringComparison.OrdinalIgnoreCase) == 0;

    public static bool IsSchemaColumn(string tableName) =>
        string.Compare(tableName, SchemaColumn, StringComparison.OrdinalIgnoreCase) == 0;

    private void JsonWatcher_Changed(object sender, FileSystemEventArgs e)
    {
        //we dont need to update anything if dataset is null
        if (DataSet == null)
        {
            return;
        }

        tablesToUpdate.Add(Path.GetFileNameWithoutExtension(e.FullPath));
    }

    public DataTable ReadFile(FileQuery.FileQuery<TFileParameter> queryParser, bool shouldLock = false)
    {
        DataTable returnValue;

        if (shouldLock)
            FileWriter<TFileParameter>._rwLock.EnterReadLock();

        try
        {
            EnsureFileSystemWatcher();

            if (fileConnection.FolderAsDatabase)
            {
                DataSet ??= new DataSet();
                var newTables = GetTableNames(queryParser);

                if (newTables.Count == 1)
                {
                    //If table is INFORMATION_SCHEMA.TABLES, we don't need to load all the tables, because the
                    //names of the tables are just files on disk and we can just do a Directory.GetFiles() to
                    //get their names.
                    var newTableName = newTables.First();
                    if (IsSchemaTable(newTableName))
                        return GenerateInformationSchemaTable();

                    //If table is INFORMATION_SCHEMA.COLUMNS, we need to load all tables, because the column
                    //information is within each table file.
                    if (IsSchemaColumn(newTableName))
                        newTables = GetTableNamesFromFolderAsDatabase().ToHashSet();
                }

                //Be explicit about not supporting INFORMATION_SCHEMA.TABLES in JOINs yet.
                if (newTables.Any(tableName => IsSchemaTable(tableName)))
                    throw new ArgumentException($"This provider does not support using the {SchemaTable} table in a JOIN.");

                //Be explicit about not supporting INFORMATION_SCHEMA.COLUMNS in JOINs yet.
                if (newTables.Any(tableName => IsSchemaColumn(tableName)))
                    throw new ArgumentException($"This provider does not support using the {SchemaColumn} table in a JOIN.");

                ReadFromFolder(newTables.Where(x => DataSet.Tables[x] == null));

            }
            else if (DataSet == null)
            {
                ReadFromFile();
            }

            //Reload any of the tables from disk?
            CheckForTableReload();

            returnValue = CheckIfSelect(queryParser);
        }
        finally
        {
            if (shouldLock)
                FileWriter<TFileParameter>._rwLock.ExitReadLock();
        }
        return returnValue;
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

    private DataTable CheckIfSelect(FileQuery.FileQuery<TFileParameter> jsonQueryParser)
    {
        if (jsonQueryParser is FileSelectQuery<TFileParameter> jsonSelectQuery)
        {
            //Parser is JsonSelectQuery

            var dataTableJoin = jsonSelectQuery.GetFileJoin();
            if (dataTableJoin == null)
            {
                //No table join.
                return GetDataTable(jsonQueryParser.TableName)!;
            }

            //NOTE:  No support yet for INFORMATION_SCHEMA.TABLES table in SQL queries that have JOIN
            return dataTableJoin.Join(DataSet!);
        }

        //Parser is not a JsonSelectQuery
        return GetDataTable(jsonQueryParser!.TableName)!;
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
            AddRowToSchemaTable(informationSchemaTable, tableName);
        }

        return informationSchemaTable;
    }

    private void AddRowToSchemaTable(DataTable schemaTable, string tableName)
    {
        var row = schemaTable.NewRow();
        row["TABLE_CATALOG"] = Path.GetFileNameWithoutExtension(fileConnection.Database);
        row["TABLE_NAME"] = tableName;
        row["TABLE_TYPE"] = "BASE TABLE";
        schemaTable.Rows.Add(row);
    }


    private DataTable GenerateInformationSchemaColumn()
    {
        var databaseName = Path.GetFileNameWithoutExtension(fileConnection.Database);
        var informationSchemaColumn = new DataTable(SchemaColumn);

        // Add columns to the DataTable
        informationSchemaColumn.Columns.Add("TABLE_CATALOG", typeof(string));
        informationSchemaColumn.Columns.Add("TABLE_NAME", typeof(string));
        informationSchemaColumn.Columns.Add("COLUMN_NAME", typeof(string));
        informationSchemaColumn.Columns.Add("DATA_TYPE", typeof(string));

        //Get all table colums sorted in order of the primary key (database, table, column)
        var allTableColumns = DataSet.Tables.Cast<DataTable>()
            .SelectMany(table => table.Columns.Cast<DataColumn>().Select(column => (Table: table, Column: column)))
            .OrderBy(tuple => tuple.Table.TableName).ThenBy(tuple => tuple.Column.ColumnName).ToList();

        //TODO: When we upgrade to C# 12, use specify an alias for tuple type with the using directive.  REF:  https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/value-tuples
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


    private DataTable GetDataTable(string tableName)
    {
        if (IsSchemaTable(tableName))
            return GenerateInformationSchemaTable();

        if (IsSchemaColumn(tableName))
            return GenerateInformationSchemaColumn();

        return DataSet!.Tables[tableName]!;
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
  

    private HashSet<string> GetTableNames(FileQuery.FileQuery<TFileParameter> jsonQueryParser)
    {
        //Start with the name of the first table in the JOIN
        var tableNames = new HashSet<string> { jsonQueryParser!.TableName };

        //If this is a SELECT with JOINs and is directory-based storage 
        if (jsonQueryParser is FileSelectQuery<TFileParameter> FileSelectQuery && FileSelectQuery.GetFileJoin() != null && fileConnection.FolderAsDatabase)
        {
            foreach (string jsonFile in GetTableNamesFromFolderAsDatabase().Where(x => x.ToLower() != jsonQueryParser.TableName.ToLower()))
            {
                tableNames.Add(jsonFile);
            }
        }

        return tableNames;
    }

    private IEnumerable<string> GetFilesFromFolderAsDatabase() => fileConnection.FolderAsDatabase ?
        Directory.GetFiles(fileConnection.Database, $"*.{fileConnection.FileExtension}") :
        throw new ArgumentException($"The file connection for {GetType()} doesn't have a DataSource which is a folder.");

    private IEnumerable<string> GetTableNamesFromFolderAsDatabase() => GetFilesFromFolderAsDatabase().Select(x => Path.GetFileNameWithoutExtension(x));

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
