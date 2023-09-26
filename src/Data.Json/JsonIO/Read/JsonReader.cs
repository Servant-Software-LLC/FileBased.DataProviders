using System.Data.JsonClient;

namespace Data.Json.JsonIO.Read;

internal class JsonReader : FileReader
{
    public JsonReader(JsonConnection jsonConnection) 
        : 
        base(jsonConnection)
    {
    }

    private JsonDocument Read(string path)
    {
        //ThrowHelper.ThrowIfInvalidPath(path);
        using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
            return JsonDocument.Parse(stream);
        }
    }

    protected override void ReadFromFolder(string tableName)
    {
        var path = fileConnection.GetTablePath(tableName);
        using JsonDocument doc = Read(path);
        var element = doc.RootElement;
        JsonException.ThrowHelper.ThrowIfInvalidJson(element, fileConnection);
        var dataTable = CreateNewDataTable(element);
        dataTable.TableName = tableName;
        Fill(dataTable, element);
        DataSet!.Tables.Add(dataTable);
    }

    protected override void UpdateFromFolder(string tableName)
    {
        var path = fileConnection.GetTablePath(tableName);
        using JsonDocument doc = Read(path);
        var element = doc.RootElement;
        JsonException.ThrowHelper.ThrowIfInvalidJson(element, fileConnection);
        var dataTable = DataSet!.Tables[tableName];
        if (dataTable == null)
        {
            dataTable = CreateNewDataTable(element);
            dataTable.TableName = tableName;
            DataSet!.Tables.Add(dataTable);
        }
        dataTable!.Clear();
        Fill(dataTable, element);
    }

    protected override void ReadFromFile()
    {
        using JsonDocument doc = Read(fileConnection.Database);
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

        using JsonDocument doc = Read(fileConnection.Database);
        var element = doc.RootElement;
        Json.JsonException.ThrowHelper.ThrowIfInvalidJson(element, fileConnection);
        foreach (DataTable item in DataSet.Tables)
        {
            var jsonElement = element.GetProperty(item.TableName);
            Fill(item, jsonElement);
        }
        doc.Dispose();

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
        return enumerator.Select(x => (x.Name, x.Value.ValueKind.GetClrFieldType()));
    }

    private void Fill(DataTable dataTable, JsonElement jsonElement)
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
