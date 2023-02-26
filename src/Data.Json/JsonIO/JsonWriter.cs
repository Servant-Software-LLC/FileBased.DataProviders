namespace Data.Json.JsonIO;

public abstract class JsonWriter
{
    private readonly JsonConnection jsonConnection;
    internal readonly JsonCommand jsonCommand;
    protected readonly JsonQuery.JsonQuery jsonQuery;
    protected readonly JsonReader jsonReader;
    private readonly JsonTransaction? jsonTransaction;

    public JsonWriter(JsonConnection jsonConnection,
        JsonCommand jsonCommand
        ,JsonQuery.JsonQuery jsonQuery)
    {
        this.jsonConnection = jsonConnection;
        this.jsonCommand = jsonCommand;
        this.jsonQuery = jsonQuery;
        this.jsonReader = jsonConnection.JsonReader;
        this.jsonTransaction=jsonCommand.Transaction as JsonTransaction;
    }

    public abstract int Execute();    
    internal static ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim();

    public virtual bool Save()
    {
        if (jsonTransaction!=null&&jsonTransaction?.TransactionDone==false)
        {
            jsonTransaction.Writers.Add(this);
            return true;
        }
        if (jsonConnection.PathType == Enum.PathType.Directory)
        {
            SaveFolderAsDB(jsonQuery.TableName);
        }
        else
        {
            SaveToFile();
        }

        return true;
    }
    private void SaveToFile()
    {
        using (var fileStream = new FileStream(jsonConnection.Database, FileMode.Create, FileAccess.Write))
        using (var jsonWriter = new Utf8JsonWriter(fileStream, new JsonWriterOptions() { Indented = jsonConnection.Formatted }))
        {
            jsonWriter.WriteStartObject();
            foreach (DataTable table in jsonReader.DataSet!.Tables)
            {
                WriteTable(jsonWriter, table, true);
            }
            jsonWriter.WriteEndObject();
        }
    }
    private  void SaveFolderAsDB(string? tableName)
    {
        var tablesToWrite = jsonConnection.JsonReader.DataSet!.Tables.Cast<DataTable>();
        if (!string.IsNullOrEmpty(tableName))
            tablesToWrite = tablesToWrite.Where(t => t.TableName == tableName);

        foreach (DataTable table in tablesToWrite)
        {
            var path = jsonConnection.GetTablePath(table.TableName);
            using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
            using (var jsonWriter = new Utf8JsonWriter(fileStream, new JsonWriterOptions() { Indented = jsonConnection.Formatted }))
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
