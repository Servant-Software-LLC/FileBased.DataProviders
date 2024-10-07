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
        DataSet!.Tables.Add(dataTable);
    }

    protected override void UpdateFromFolder(string tableName)
    {
        // Read the json file
        using JsonDocument doc = Read(tableName);
        var element = doc.RootElement;
        JsonException.ThrowHelper.ThrowIfInvalidJson(element, fileConnection);

        // Determine if the table exists in the DataSet, if not create it.
        var dataTable = DataSet!.Tables[tableName];
        if (dataTable == null)
        {
            dataTable = CreateNewDataTable(element);
            dataTable.TableName = tableName;
            DataSet!.Tables.Add(dataTable);
        }

        // Clear the table and fill it with the new data
        dataTable!.Clear();
        Fill(dataTable, element);
    }

    protected override void ReadFromFile()
    {
        using JsonDocument doc = Read(string.Empty);
        var element = doc.RootElement;
        JsonException.ThrowHelper.ThrowIfInvalidJson(element, fileConnection);
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

    protected override void UpdateFromFile()
    {
        DataSet!.Clear();

        using JsonDocument doc = Read(string.Empty);
        var element = doc.RootElement;
        Json.JsonException.ThrowHelper.ThrowIfInvalidJson(element, fileConnection);
        foreach (DataTable item in DataSet.Tables)
        {
            var jsonElement = element.GetProperty(item.TableName);
            Fill(item, jsonElement);
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

    private static IEnumerable<(string name, Type type)> GetFields(JsonElement table)
    {
        var arrayEnumerator = table.EnumerateArray();
        if (!arrayEnumerator.Any())
            return Enumerable.Empty<(string name, Type type)>();

        var maxFieldElement = arrayEnumerator.MaxBy(x =>
        {
            return x.EnumerateObject().Count();
        });
        var enumerator = maxFieldElement.EnumerateObject();
        return enumerator.Select(x => (x.Name, x.Value.ValueKind.GetClrFieldType()));
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

    private static DataTable CreateNewDataTable(JsonElement jsonElement)
    {
        DataTable dataTable = new DataTable();
        foreach (var col in GetFields(jsonElement))
        {
            dataTable.Columns.Add(col.name, col.type);
        }

        return dataTable;
    }

}
