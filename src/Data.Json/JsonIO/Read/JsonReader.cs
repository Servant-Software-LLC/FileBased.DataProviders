using Data.Json.Enum;
using Data.Json.JsonQuery;

namespace Data.Json.JsonIO.Read;

public class JsonReader : IDisposable
{
    private FileSystemWatcher? jsonWatcher;
    private readonly JsonConnection jsonConnection;
    private readonly HashSet<string> tablesToUpdate = new();


    public DataSet? DataSet { get; private set; }

    public JsonReader(JsonConnection jsonConnection)
    {
        this.jsonConnection = jsonConnection ?? throw new ArgumentNullException(nameof(jsonConnection));

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

    private JsonDocument Read(string path)
    {
        //ThrowHelper.ThrowIfInvalidPath(path);
        using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
            return JsonDocument.Parse(stream);
        }
    }


    public DataTable ReadJson(JsonQuery.JsonQuery jsonQueryParser, bool shouldLock = false)
    {
        DataTable returnValue;

        if (shouldLock)
            JsonWriter._rwLock.EnterReadLock();

        try
        {
            EnsureFileSystemWatcher();

            if (jsonConnection.PathType == PathType.Directory)
            {
                DataSet ??= new DataSet();
                var newTables = GetTableNames(jsonQueryParser);
                ReadFromFolder(newTables.Where(x => DataSet.Tables[x] == null));
                returnValue = CheckIfSelect(jsonQueryParser);
            }
            else
            {
                //Load json database file from disk for the first time?
                if (DataSet == null)
                {
                    ReadFromFile();
                }

                returnValue = CheckIfSelect(jsonQueryParser);
            }

            //Reload any of the tables from disk?
            returnValue = CheckForTableReload(jsonQueryParser, returnValue);

        }
        finally
        {
            if (shouldLock)
                JsonWriter._rwLock.ExitReadLock();
        }
        return returnValue;
    }

    private DataTable CheckForTableReload(JsonQuery.JsonQuery jsonQueryParser, DataTable returnValue)
    {
        if (tablesToUpdate.Count > 0)
        {
            if (jsonConnection.PathType == PathType.File)
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
            returnValue = CheckIfSelect(jsonQueryParser);
        }

        return returnValue;
    }

    private DataTable CheckIfSelect(JsonQuery.JsonQuery jsonQueryParser)
    {
        if (jsonQueryParser is JsonSelectQuery jsonSelectQuery)
        {
            var dataTableJoin = jsonSelectQuery.GetJsonJoin();
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
            if (jsonConnection.PathType == PathType.Directory)
            {
                jsonWatcher.Path = jsonConnection.Database;
                jsonWatcher.Filter = "*.json";
            }
            else
            {
                var file = new FileInfo(jsonConnection.Database);
                jsonWatcher.Path = file.DirectoryName!;
                jsonWatcher.Filter = file.Name;
            }
            jsonWatcher.EnableRaisingEvents = true;
        }
    }


    private HashSet<string> GetTableNames(JsonQuery.JsonQuery jsonQueryParser)
    {
        //Start with the name of the first table in the JOIN
        var tableNames = new HashSet<string> { jsonQueryParser!.TableName };

        //If this is a SELECT with JOINs and is directory-based storage 
        if (jsonQueryParser is JsonSelectQuery jsonSelectQuery && jsonSelectQuery.GetJsonJoin() != null && jsonConnection.PathType == PathType.Directory)
        {
            string[] jsonFiles = Directory.GetFiles(jsonConnection.Database, "*.json");

            foreach (string jsonFile in jsonFiles.Select(x => Path.GetFileNameWithoutExtension(x)).Where(x => x != jsonQueryParser.TableName))
            {
                tableNames.Add(jsonFile);
            }
        }

        return tableNames;
    }

    private void ReadFromFolder(IEnumerable<string> tableNames)
    {

        foreach (var name in tableNames)
        {
            var path = jsonConnection.GetTablePath(name);
            var doc = Read(path);
            var element = doc.RootElement;
            ThrowHelper.ThrowIfInvalidJson(element, jsonConnection);
            var dataTable = CreateNewDataTable(element);
            dataTable.TableName = name;
            Fill(dataTable, element);
            DataSet!.Tables.Add(dataTable);
            doc.Dispose();
        }

    }

    private void UpdateFromFolder(string tableName)
    {
        var path = jsonConnection.GetTablePath(tableName);
        var doc = Read(path);
        var element = doc.RootElement;
        ThrowHelper.ThrowIfInvalidJson(element, jsonConnection);
        var dataTable = DataSet!.Tables[tableName];
        if (dataTable == null)
        {
            dataTable = CreateNewDataTable(element);
            dataTable.TableName = tableName;
            DataSet!.Tables.Add(dataTable);
        }
        dataTable!.Clear();
        Fill(dataTable, element);
        doc.Dispose();

    }

    #region File Read Update
    private void ReadFromFile()
    {
        var doc = Read(jsonConnection.Database);
        var element = doc.RootElement;
        ThrowHelper.ThrowIfInvalidJson(element, jsonConnection);
        var dataBaseEnumerator = element.EnumerateObject();
        DataSet = new DataSet();
        foreach (var item in dataBaseEnumerator)
        {
            var dataTable = CreateNewDataTable(item.Value);
            dataTable.TableName = item.Name;
            Fill(dataTable, item.Value);
            DataSet.Tables.Add(dataTable);
        }
        doc.Dispose();

    }
    private void UpdateFromFile()
    {
        DataSet!.Clear();

        var doc = Read(jsonConnection.Database);
        var element = doc.RootElement;
        ThrowHelper.ThrowIfInvalidJson(element, jsonConnection);
        foreach (DataTable item in DataSet.Tables)
        {
            var jsonElement = element.GetProperty(item.TableName);
            Fill(item, jsonElement);
        }
        doc.Dispose();

    }
    #endregion


    DataTable CreateNewDataTable(JsonElement jsonElement)
    {
        DataTable dataTable = new DataTable();
        foreach (var col in GetFields(jsonElement))
        {
            dataTable.Columns.Add(col.name, col.type);
        }

        return dataTable;
    }
    public DataTable GetDataTable(JsonElement jsonElement)
    {
        //create datatable
        var dataTable = new DataTable();
        Fill(dataTable, jsonElement);
        return dataTable;

    }
    public IEnumerable<(string name, Type type)> GetFields(JsonElement table)
    {
        var maxFieldElement = table.EnumerateArray().MaxBy(x =>
        {
            return x.EnumerateObject().Count();
        });
        var enumerator = maxFieldElement.EnumerateObject();
        return enumerator.Select(x => (x.Name, x.Value.ValueKind.GetClrFieldType()));
    }
    internal void Fill(DataTable dataTable, JsonElement jsonElement)
    {
        //fill datatables
        foreach (var row in jsonElement.EnumerateArray())
        {
            var newRow = dataTable.NewRow();
            foreach (var field in row.EnumerateObject())
            {
                var val = field.Value.GetValue();
                if (val != null)
                    newRow[field.Name] = val;
            }
            dataTable.Rows.Add(newRow);
        }

    }

    public void Dispose()
    {
        //_jsonWatcher.Dispose();
        //DataSet.Dispose();
    }
}
