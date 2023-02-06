using Bogus.DataSets;
using Data.Json.Enum;
using Data.Json.JsonJoin;
using Data.Json.JsonQuery;
using System;
using System.Collections.Generic;
using System.Data.JsonClient;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Data.Json.New
{
    internal class Reader:IDisposable
    {
        JsonConnection JsonConnection { get; }
        public JsonQueryParser? JsonQueryParser { get;internal set; }

        private FileSystemWatcher _jsonWatcher;
        public DataSet? DataSet { get; set; }
        public DataTable DataTable { get; set; }
        public Reader(JsonConnection jsonConnection)
        {
            JsonConnection = jsonConnection;
           
            DataSet = null;
            
        }
        
        public void StartWatching()
        { 
            _jsonWatcher.Changed -= JsonWatcher_Changed;
            _jsonWatcher.Changed += JsonWatcher_Changed;
        }
        public void StopWatching()
        {
            _jsonWatcher.Changed -= JsonWatcher_Changed;
        }
        public int FieldCount
        {
            get
            {
                ReadJson();
                return DataTable.Columns.Count;
            }
        }

        bool _shouldUpdate = false;
        List<string> tables =
            new List<string>();
        private void JsonWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            //we dont need to update anything if dataset is null
            if (DataSet==null)
            {
                return;
            }
            _shouldUpdate = true;
            tables.Add(Path.GetFileNameWithoutExtension(e.FullPath));
        
        }
        private JsonDocument Read(string path)
        {
            //ThrowHelper.ThrowIfInvalidPath(path);
            using var stream = new FileStream(path,FileMode.Open,FileAccess.Read);
            return JsonDocument.Parse(stream);
        }

        public void ReadJson()
        {

            if (_jsonWatcher == null)
            {
                _jsonWatcher = new FileSystemWatcher();
                _jsonWatcher.NotifyFilter = NotifyFilters.LastWrite;
                if (JsonConnection.PathType == PathType.Directory)
                {
                    _jsonWatcher.Path = JsonConnection.ConnectionString;
                    _jsonWatcher.Filter = "*.json";
                }
                else
                {
                    var file = new FileInfo(JsonConnection.ConnectionString);
                    _jsonWatcher.Path = file.DirectoryName!;
                    _jsonWatcher.Filter = file.Name;
                }
                //  _jsonWatcher.EnableRaisingEvents= true;
            }
            if (JsonConnection.PathType == PathType.Directory)
            {
                DataSet ??= new DataSet();
                var newTables = GetTables();
                ReadFromFolder(newTables.Where(x => DataSet.Tables[x] == null));
                DataTable = DataSet.Tables[JsonQueryParser!.Table]!;
                CheckIfSelect();
            }
            else
            {
                if (DataSet == null)
                {
                    ReadFromFile();
                    CheckIfSelect();
                }
            }

            if (_shouldUpdate)
            {
                if (JsonConnection.PathType == PathType.File)
                {
                    UpdateFromFile();
                }
                else
                {
                    foreach (var item in tables)
                    {
                        UpdateFromFolder(item);
                    }
                }
                tables.Clear();
                _shouldUpdate = false;
                DataTable = DataSet.Tables[JsonQueryParser.Table];
            }

            void CheckIfSelect()
            {
                if (JsonQueryParser is JsonSelectQuery jsonSelectQuery)
                {
                    var dataTableJoin = jsonSelectQuery.GetJsonJoin();
                    if (dataTableJoin == null)
                    {
                        DataTable = DataSet!.Tables[JsonQueryParser.Table]!;
                    }
                    else
                    {
                        DataTable = dataTableJoin.Join(DataSet!);

                    }
                }
            }
        }

        private List<string> GetTables()
        {
            var tables = new List<string>
                {
                    JsonQueryParser!.Table
                };
            if (JsonQueryParser is JsonSelectQuery jsonSelectQuery)
            {
                //foreach (var item in jsonSelectQuery.join.Tables)
                //{
                //    tables.Add(item);
                //}
            }

            return tables;
        }

        public string GetTablePath(string tableName)=>
             $"{JsonConnection.ConnectionString}/{tableName}.json";
        private void ReadFromFolder(IEnumerable<string> tables)
        {
            foreach (var name in tables)
            {
                var path = GetTablePath(name);
                var element = Read(path).RootElement;
                ThrowHelper.ThrowIfInvalidJson(element, JsonConnection);
                var dataTable = CreateNewDataTable(element);
                dataTable.TableName = name;
                Fill(dataTable, element);
                DataSet!.Tables.Add(dataTable);
            }
        }
        private void UpdateFromFolder(string tableName)
        {
            var path = GetTablePath(tableName);
            var element = Read(path).RootElement;
            ThrowHelper.ThrowIfInvalidJson(element, JsonConnection);
            var dataTable = DataSet!.Tables[tableName];
            dataTable!.Clear();
            Fill(dataTable, element);
        }

        #region File Read Update
        private void ReadFromFile()
        {
            while (JsonConnection.LockSlim.WaitingWriteCount>0)
            {

            }
            var element = Read(JsonConnection.Database).RootElement;
            ThrowHelper.ThrowIfInvalidJson(element, JsonConnection);
            var dataBaseEnumerator = element.EnumerateObject();
            DataSet = new DataSet();
            foreach (var item in dataBaseEnumerator)
            {
                var dataTable = CreateNewDataTable(item.Value);
                dataTable.TableName = item.Name;
                Fill(dataTable, item.Value);
                DataSet.Tables.Add(dataTable);
            }
        }
        private void UpdateFromFile()
        {
            DataSet!.Clear();
            var element = Read(JsonConnection.Database).RootElement;
            ThrowHelper.ThrowIfInvalidJson(element, JsonConnection);
            foreach (DataTable item in DataSet.Tables)
            {
                var jsonElement = element.GetProperty(item.TableName);
                Fill(item,jsonElement);
            }
        }
        #endregion


        public DataTable CreateNewDataTable(JsonElement jsonElement)
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
            Fill(dataTable,jsonElement);
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
        internal void Fill(DataTable dataTable,JsonElement jsonElement)
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
}
