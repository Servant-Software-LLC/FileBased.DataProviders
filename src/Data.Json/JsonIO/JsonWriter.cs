namespace Data.Json.JsonIO;

public abstract class JsonWriter
{
    private readonly JsonConnection jsonConnection;
    protected readonly JsonReader jsonReader;

    public JsonWriter(JsonConnection jsonConnection)
    {
        this.jsonConnection = jsonConnection;
        jsonReader = jsonConnection.JsonReader;
    }

    public abstract int Execute();    
    internal static ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim();

    public bool Save(string? tableName) => SaveData(jsonConnection, tableName);

    private static bool SaveData(JsonConnection jsonConnection, string? tableName)
    {        
        var path = jsonConnection.ConnectionString;
        if (jsonConnection.PathType == Enum.PathType.Directory)
        {
            SaveFolderAsDB(jsonConnection, tableName);
        }
        else
        {
            SaveToFile(jsonConnection);
        }
       
        return true;
    }

    private static void SaveToFile(JsonConnection jsonConnection)
    {
        using (var fileStream = new FileStream(jsonConnection.ConnectionString, FileMode.Create, FileAccess.Write))
        using (var jsonWriter = new Utf8JsonWriter(fileStream))
        {
            jsonWriter.WriteStartObject();
            foreach (DataTable table in jsonConnection.JsonReader.DataSet!.Tables)
            {
                WriteTable(jsonWriter, table, true);
            }
            jsonWriter.WriteEndObject();
        }
    }

    private static void SaveFolderAsDB(JsonConnection jsonConnection, string? tableName)
    {
        var tablesToWrite = jsonConnection.JsonReader.DataSet!.Tables.Cast<DataTable>();
        if (!string.IsNullOrEmpty(tableName))
            tablesToWrite = tablesToWrite.Where(t => t.TableName == tableName);

        foreach (DataTable table in tablesToWrite)
        {
            var path = Path.Combine(jsonConnection.ConnectionString, $"{table.TableName}.json");
            using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
            using (var jsonWriter = new Utf8JsonWriter(fileStream))
            {
                WriteTable(jsonWriter, table, false);
            }
        }
    }

    private static void WriteTable(Utf8JsonWriter jsonWriter, DataTable table, bool writeTableName)
    {
        if (writeTableName)
            jsonWriter.WriteStartArray(table.TableName);
        else
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
