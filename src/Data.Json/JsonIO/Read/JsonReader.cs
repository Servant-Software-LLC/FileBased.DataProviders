using SqlBuildingBlocks.POCOs;
using System.Data.JsonClient;

namespace Data.Json.JsonIO.Read;

internal class JsonReader : FileReader
{
    public JsonReader(JsonConnection jsonConnection) 
        : 
        base(jsonConnection)
    {
    }

    protected override void ReadFromFolder(string tableName)
    {
        using JsonDocument doc = Read(tableName);
        var element = doc.RootElement;
        JsonException.ThrowHelper.ThrowIfInvalidJson(element, fileConnection);
        var dataTable = CreateNewDataTable(element);
        dataTable.TableName = tableName;
        Fill(dataTable, element);

        //TODO:  Need to support large data files (https://github.com/Servant-Software-LLC/FileBased.DataProviders/issues/83) 
        VirtualDataTable virtualDataTable = new(dataTable);
        DataSet!.Tables.Add(virtualDataTable);
    }

    protected override void UpdateFromFolder(string tableName)
    {
        // Read the json file
        using JsonDocument doc = Read(tableName);
        var element = doc.RootElement;
        JsonException.ThrowHelper.ThrowIfInvalidJson(element, fileConnection);

        // Determine if the table exists in the DataSet, if not create it.
        var virtualDataTable = DataSet!.Tables[tableName];
        if (virtualDataTable == null)
        {
            //TODO:  Need to support large data files (https://github.com/Servant-Software-LLC/FileBased.DataProviders/issues/83) 
            var dataTable = CreateNewDataTable(element);
            dataTable.TableName = tableName;
            
            virtualDataTable = new(dataTable);
            DataSet!.Tables.Add(virtualDataTable);
        }

        // Clear the virtual table's rows and fill it with the new data
        var replacementDataTable = virtualDataTable.CreateEmptyDataTable();

        //TODO:  Need to support large data files (https://github.com/Servant-Software-LLC/FileBased.DataProviders/issues/83) 
        Fill(replacementDataTable, element);
        virtualDataTable.Rows = replacementDataTable.Rows.Cast<DataRow>();
    }

    protected override void ReadFromFile()
    {
        using JsonDocument doc = Read(string.Empty);
        var element = doc.RootElement;
        JsonException.ThrowHelper.ThrowIfInvalidJson(element, fileConnection);
        var dataBaseEnumerator = element.EnumerateObject();
        DataSet = new VirtualDataSet();
        foreach (var item in dataBaseEnumerator)
        {
            var dataTable = CreateNewDataTable(item.Value);
            dataTable.TableName = item.Name;
            Fill(dataTable, item.Value);

            VirtualDataTable virtualDataTable = new(dataTable);
            DataSet.Tables.Add(virtualDataTable);
        }
    }

    protected override void UpdateFromFile()
    {
        using JsonDocument doc = Read(string.Empty);
        var element = doc.RootElement;
        JsonException.ThrowHelper.ThrowIfInvalidJson(element, fileConnection);
        foreach (VirtualDataTable virtualDataTable in DataSet.Tables)
        {
            var jsonElement = element.GetProperty(virtualDataTable.TableName);

            var dataTable = virtualDataTable.CreateEmptyDataTable();
            Fill(dataTable, jsonElement);

            virtualDataTable.Rows = dataTable.Rows.Cast<DataRow>();
        }
        doc.Dispose();

    }

    // Create a JsonDocument from the file (the file could be a database or a table)
    private JsonDocument Read(string tableName)
    {
        var jsonDocumentOptions = new JsonDocumentOptions
        {
            CommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
        };

        using (var textReader = fileConnection.DataSourceProvider.GetTextReader(tableName))
        {
            //Read the entire content from the TextReader into a string
            string jsonString = textReader.ReadToEnd();

            return JsonDocument.Parse(jsonString, jsonDocumentOptions);
        }
    }

    private IEnumerable<(string name, Type type)> GetFields(JsonElement table)
    {
        var arrayEnumerator = table.EnumerateArray();
        if (!arrayEnumerator.Any())
            return Enumerable.Empty<(string name, Type type)>();

        var maxFieldElement = arrayEnumerator.MaxBy(x =>
        {
            return x.EnumerateObject().Count();
        });
        var enumerator = maxFieldElement.EnumerateObject();
        return enumerator.Select(x => (x.Name, x.Value.ValueKind.GetClrFieldType(fileConnection)));
    }

    private static void Fill(DataTable dataTable, JsonElement jsonElement)
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

    private DataTable CreateNewDataTable(JsonElement jsonElement)
    {
        DataTable dataTable = new DataTable();
        foreach (var col in GetFields(jsonElement))
        {
            dataTable.Columns.Add(col.name, col.type);
        }

        return dataTable;
    }

}
