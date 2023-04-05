namespace Data.Common.FileIO.Read;
public abstract class FileReader : IDisposable
{
    private FileSystemWatcher? jsonWatcher;
    protected readonly FileConnection fileConnection;
    private readonly HashSet<string> tablesToUpdate = new();

    public DataSet? DataSet { get; protected set; }

    public FileReader(FileConnection fileConnection)
    {
        this.fileConnection = fileConnection ?? throw new ArgumentNullException(nameof(fileConnection));

        DataSet = null;
    }

    public void StartWatching()
    {
        if (jsonWatcher != null)
        {

            jsonWatcher.Changed -= JsonWatcher_Changed;
            jsonWatcher.Changed += JsonWatcher_Changed;
        }
    }
    public void StopWatching()
    {
        if (jsonWatcher != null)
            jsonWatcher.Changed -= JsonWatcher_Changed;
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

    public DataTable ReadFile(FileQuery.FileQuery queryParser, bool shouldLock = false)
    {
        DataTable returnValue;

        if (shouldLock)
            FileWriter._rwLock.EnterReadLock();

        try
        {
            EnsureFileSystemWatcher();

            if (fileConnection.PathType == PathType.Directory)
            {
                DataSet ??= new DataSet();
                var newTables = GetTableNames(queryParser);
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
                FileWriter._rwLock.ExitReadLock();
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

    private DataTable CheckIfSelect(FileQuery.FileQuery jsonQueryParser)
    {
        if (jsonQueryParser is FileSelectQuery jsonSelectQuery)
        {
            var dataTableJoin = jsonSelectQuery.GetFileJoin();
            if (dataTableJoin == null)
            {
                return DataSet!.Tables[jsonQueryParser.TableName]!;
            }

            //No table join.
            return dataTableJoin.Join(DataSet!);
        }

        //Parser is not a JsonSelectQuery
        return DataSet!.Tables[jsonQueryParser!.TableName]!;
    }

    private void EnsureFileSystemWatcher()
    {
        if (jsonWatcher == null)
        {
            jsonWatcher = new FileSystemWatcher();
            jsonWatcher.NotifyFilter = NotifyFilters.LastWrite;
            if (fileConnection.PathType == PathType.Directory)
            {
                jsonWatcher.Path = fileConnection.Database;
                jsonWatcher.Filter = $"*.{fileConnection.FileExtension}";
            }
            else
            {
                var file = new FileInfo(fileConnection.Database);
                jsonWatcher.Path = file.DirectoryName!;
                jsonWatcher.Filter = file.Name;
            }
            jsonWatcher.EnableRaisingEvents = true;
        }
    }
  

    private HashSet<string> GetTableNames(FileQuery.FileQuery jsonQueryParser)
    {
        //Start with the name of the first table in the JOIN
        var tableNames = new HashSet<string> { jsonQueryParser!.TableName };

        //If this is a SELECT with JOINs and is directory-based storage 
        if (jsonQueryParser is FileSelectQuery FileSelectQuery && FileSelectQuery.GetFileJoin() != null && fileConnection.PathType == PathType.Directory)
        {
            string[] jsonFiles = Directory.GetFiles(fileConnection.Database, $"*.{fileConnection.FileExtension}");

            foreach (string jsonFile in jsonFiles.Select(x => Path.GetFileNameWithoutExtension(x)).Where(x => x.ToLower() != jsonQueryParser.TableName.ToLower()))
            {
                tableNames.Add(jsonFile);
            }
        }

        return tableNames;
    }

    protected abstract void ReadFromFolder(IEnumerable<string> tableNames);
    protected abstract void UpdateFromFolder(string tableName);
    protected abstract void ReadFromFile();
    protected abstract void UpdateFromFile();
   
    public void Dispose()
    {
        //_jsonWatcher.Dispose();
        //DataSet.Dispose();
    }
}
