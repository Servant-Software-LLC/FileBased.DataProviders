namespace System.Data.JsonClient;

public class JsonDataAdapter : DbDataAdapter, IDataAdapter
{
    public JsonCommand SelectCommand { get; set; }
    public JsonCommand UpdateCommand { get; set; }

    public MissingMappingAction MissingMappingAction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public MissingSchemaAction MissingSchemaAction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public ITableMappingCollection TableMappings => throw new NotImplementedException();

    public int Fill(DataSet dataSet)
    {
        if (SelectCommand == null)
        {
            throw new InvalidOperationException("SelectCommand is not set.");
        }

        var jsonDoc = JsonDocument.Parse(SelectCommand.Connection.Database);
        var jsonData = jsonDoc.RootElement;

        var filter = SelectCommand.CommandText.Split(" ")[1];

        if (jsonData.TryGetProperty(filter, out JsonElement value))
            jsonData = value;
        else
            throw new InvalidOperationException("Invalid filter");

        if (jsonData.ValueKind == JsonValueKind.Array)
        {
            var dataTable = new DataTable();
            var first = jsonData.EnumerateArray().First();
            foreach (var property in first.EnumerateObject())
            {
                dataTable.Columns.Add(property.Name);
            }
            foreach (var element in jsonData.EnumerateArray())
            {
                var row = dataTable.NewRow();
                foreach (var property in element.EnumerateObject())
                {
                    row[property.Name] = property.Value.GetRawText();
                }
                dataTable.Rows.Add(row);
            }
            dataSet.Tables.Add(dataTable);
        }
        else
        {
            var dataTable = new DataTable();
            foreach (var property in jsonData.EnumerateObject())
            {
                dataTable.Columns.Add(property.Name);
            }
            var row = dataTable.NewRow();
            foreach (var property in jsonData.EnumerateObject())
            {
                row[property.Name] = property.Value.GetRawText();
            }
            dataTable.Rows.Add(row);
            dataSet.Tables.Add(dataTable);
        }
        return 1;
    }

    public DataTable[] FillSchema(DataSet dataSet, SchemaType schemaType)
    {
        if (SelectCommand == null)
        {
            throw new InvalidOperationException("SelectCommand is not set.");
        }
        var jsonDoc = JsonDocument.Parse(SelectCommand.Connection.Database);
        var jsonData = jsonDoc.RootElement;
        var filter = SelectCommand.CommandText.Split(" ")[1];

        if (jsonData.TryGetProperty(filter, out JsonElement value))
            jsonData = value;
        else
            throw new InvalidOperationException("Invalid filter");

        var dataTable = new DataTable();

        if (jsonData.ValueKind == JsonValueKind.Array)
        {
            var first = jsonData.EnumerateArray().First();
            foreach (var property in first.EnumerateObject())
            {
                dataTable.Columns.Add(property.Name);
            }
        }
        else
        {
            foreach (var property in jsonData.EnumerateObject())
            {
                dataTable.Columns.Add(property.Name);
            }
        }

        if (schemaType == SchemaType.Mapped)
        {
            dataSet.Tables.Add(dataTable);
        }
        return new DataTable[] { dataTable };
    }

    public IDataParameter[] GetFillParameters()
    {
        throw new NotImplementedException();
    }


    public int Update(DataSet dataSet)
    {
        var dataTable = dataSet.Tables[0];
        var filePath = UpdateCommand.Connection.ConnectionString;
        // Read json file into JsonDocument
        using (FileStream fs = File.OpenRead(filePath))
        using (StreamReader sr = new StreamReader(fs))
        {
            string jsonText = sr.ReadToEnd();
            var jsonDoc = JsonDocument.Parse(jsonText);
            var jsonData = jsonDoc.RootElement;
            var jsonNode = jsonData.GetProperty(UpdateCommand.CommandText);
            // Modify properties of the specific node based on the data from DataTable
            foreach (var column in dataTable.Columns)
            {
                var property = dataTable.Rows[0][column.ToString()];
                //jsonNode.WriteTo(property);
            }
            // Serialize the object back to json
            var json = jsonDoc.ToString();
            // Write json to the file
            File.WriteAllText(filePath, json);
        }
        return 1;
    }


}