namespace Data.Json.JsonIO;

internal class JsonDataSetWriter : IDataSetWriter
{
    private readonly IFileConnection fileConnection;
    private readonly FileStatement fileQuery;

    public JsonDataSetWriter(IFileConnection fileConnection, FileStatement fileQuery)
    {
        this.fileConnection = fileConnection;
        this.fileQuery = fileQuery;
    }

    public void WriteDataSet(DataSet dataSet)
    {
        if (fileConnection.PathType == PathType.Directory)
        {
            SaveFolderAsDB(fileQuery.TableName, dataSet);
        }
        else
        {
            SaveToFile(dataSet);
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

    private void SaveToFile(DataSet dataSet)
    {
        using (var fileStream = new FileStream(fileConnection.Database, FileMode.Create, FileAccess.Write))
        using (var jsonWriter = new Utf8JsonWriter(fileStream, new JsonWriterOptions() { Indented = fileConnection.Formatted ?? false}))
        {
            jsonWriter.WriteStartObject();
            foreach (DataTable table in dataSet!.Tables)
            {
                WriteTable(jsonWriter, table, true);
            }
            jsonWriter.WriteEndObject();
        }
    }

    private void SaveFolderAsDB(string? tableName, DataSet dataSet)
    {
        var tablesToWrite = dataSet!.Tables.Cast<DataTable>();
        if (!string.IsNullOrEmpty(tableName))
            tablesToWrite = tablesToWrite.Where(t => t.TableName == tableName);

        foreach (DataTable table in tablesToWrite)
        {
            var path = fileConnection.GetTablePath(table.TableName);
            using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
            using (var jsonWriter = new Utf8JsonWriter(fileStream, new JsonWriterOptions() { Indented = fileConnection.Formatted ?? false }))
            {
                WriteTable(jsonWriter, table, false);
            }
        }
    }
}
