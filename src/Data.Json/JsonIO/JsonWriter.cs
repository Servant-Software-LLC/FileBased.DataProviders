namespace Data.Json.JsonIO;

internal abstract class JsonWriter
{
    protected readonly JsonCommand command;
    protected readonly JsonConnection jsonConnection;

    public JsonReader JsonReader { get; }

    public JsonWriter(JsonCommand command, JsonConnection jsonConnection)
    {
        this.command = command;
        this.jsonConnection = jsonConnection;
        JsonReader = jsonConnection.JsonReader;
        jsonConnection.JsonReader.JsonQueryParser = command.QueryParser;
    }
    public abstract int Execute();
    internal static ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim();

    private static bool SaveData(JsonConnection jsonConnection)
    {        
        var path = jsonConnection.ConnectionString;
        if (jsonConnection.PathType == Enum.PathType.Directory)
        {
            SaveFolderAsDB(jsonConnection);
        }
        else
        {
            SaveToFile(jsonConnection);
        }
       
        return true;
    }

    public bool Save() => SaveData(jsonConnection);

    private static void SaveToFile(JsonConnection jsonConnection)
    {
        using (var fileStream = new FileStream(jsonConnection.ConnectionString, FileMode.Create, FileAccess.Write))
        using (var jsonWriter = new Utf8JsonWriter(fileStream))
        {
            jsonWriter.WriteStartObject();
            foreach (DataTable table in jsonConnection.JsonReader.DataSet!.Tables)
            {
                jsonWriter.WriteStartArray(table.TableName);
                foreach (DataRow row in table.Rows)
                {
                    jsonWriter.WriteStartObject();
                    foreach (DataColumn column in table.Columns)
                    {
                        var dataType = column.DataType.Name;
                        if (row.IsNull(column.ColumnName))
                        {
                            dataType = "Null";
                        }
                        switch (dataType)
                        {
                            case "Decimal":
                                jsonWriter.WriteNumber(column.ColumnName, (decimal)row[column]);
                                break;
                            case "String":
                                jsonWriter.WriteString(column.ColumnName, row[column].ToString().AsSpan());
                                break;
                            case "Boolean":
                                jsonWriter.WriteBoolean(column.ColumnName, (bool)row[column]);
                                break;
                            case "Null":
                                jsonWriter.WriteNull(column.ColumnName);
                                break;
                            default:
                                throw new NotSupportedException($"Data type {column.DataType.Name} is not supported.");
                        }

                        //jsonWriter.WriteString(column.ColumnName, row[column].ToString());
                    }
                    jsonWriter.WriteEndObject();
                }
                jsonWriter.WriteEndArray();
            }
            jsonWriter.WriteEndObject();
        }
    }
    private static void SaveFolderAsDB(JsonConnection jsonConnection)
    {
        foreach (DataTable table in jsonConnection.JsonReader.DataSet!.Tables)
        {
            var path = Path.Combine(jsonConnection.ConnectionString, $"{table.TableName}.json");
            using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
            using (var jsonWriter = new Utf8JsonWriter(fileStream))
            {
                jsonWriter.WriteStartArray();
                foreach (DataRow row in table.Rows)
                {
                    jsonWriter.WriteStartObject();
                    foreach (DataColumn column in table.Columns)
                    {
                        var dataType = column.DataType.Name;
                        if (row.IsNull(column.ColumnName))
                        {
                            dataType = "Null";
                        }
                        switch (dataType)
                        {
                            case "Decimal":
                                jsonWriter.WriteNumber(column.ColumnName, (decimal)row[column]);
                                break;
                            case "String":
                                jsonWriter.WriteString(column.ColumnName, row[column].ToString().AsSpan());
                                break;
                            case "Boolean":
                                jsonWriter.WriteBoolean(column.ColumnName, (bool)row[column]);
                                break;
                            case "Null":
                                jsonWriter.WriteNull(column.ColumnName);
                                break;
                            default:
                                throw new NotSupportedException($"Data type {column.DataType.Name} is not supported.");
                        }

                        //jsonWriter.WriteString(column.ColumnName, row[column].ToString());
                    }
                    jsonWriter.WriteEndObject();
                }
                jsonWriter.WriteEndArray();
            }
        }
    }
}
