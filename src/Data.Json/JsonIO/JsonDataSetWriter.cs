using Microsoft.Extensions.Logging;
using System.Text;

namespace Data.Json.JsonIO;

public class JsonDataSetWriter : IDataSetWriter
{
    private ILogger<JsonDataSetWriter> log => fileConnection.LoggerServices.CreateLogger<JsonDataSetWriter>();
    private readonly IFileConnection fileConnection;
    private readonly FileStatement fileStatement;

    public JsonDataSetWriter(IFileConnection fileConnection, FileStatement fileStatement)
    {
        this.fileConnection = fileConnection;
        this.fileStatement = this.fileStatement;
    }

    public void WriteDataSet(DataSet dataSet)
    {
        if (fileConnection.PathType == PathType.Directory)
        {
            SaveFolderAsDB(fileStatement.FromTable.TableName, dataSet);
        }
        else
        {
            SaveToFile(dataSet);
        }
    }

    /// <summary>
    /// Provides an entry point for a derived class to add a comment
    /// </summary>
    /// <param name="jsonWriter"></param>
    protected virtual void WriteCommentValue(Utf8JsonWriter jsonWriter, DataColumn column) { }

    private void WriteTable(Utf8JsonWriter jsonWriter, DataTable table, bool writeTableName)
    {
        log.LogDebug($"Writing array start.");
        if (writeTableName)
            jsonWriter.WriteStartArray(table.TableName);
        else
            jsonWriter.WriteStartArray();

        foreach (DataRow row in table.Rows)
        {
            log.LogDebug($"Writing object start for DataRow.");
            jsonWriter.WriteStartObject();
            foreach (DataColumn column in table.Columns)
            {
                var dataType = column.DataType.Name;
                log.LogDebug($"Column: {column.ColumnName}. Data type: {dataType}");

                //Provide an entry point for a derived class to add a comment
                WriteCommentValue(jsonWriter, column);

                if (row.IsNull(column.ColumnName))
                {
                    dataType = "Null";
                }
                switch (dataType)
                {
                    case "Int32":
                        jsonWriter.WriteNumber(column.ColumnName, (int)row[column]);
                        break;
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
                        throw new NotSupportedException($"Data type {dataType} is not supported.");
                }

                log.LogDebug($"Value written to jsonWriter.");
            }

            log.LogDebug($"Writing object end for DataRow.");
            jsonWriter.WriteEndObject();
        }

        log.LogDebug($"Writing array start.");
        jsonWriter.WriteEndArray();
    }

    private void SaveToFile(DataSet dataSet)
    {
        try
        {
            log.LogDebug($"{GetType()}.{nameof(SaveToFile)}(). Saving file {fileConnection.Database}");

            string jsonString;
            using (var stream = new MemoryStream())
            {
                using (var jsonWriter = new Utf8JsonWriter(stream, new JsonWriterOptions() { Indented = fileConnection.Formatted ?? false }))
                {
                    jsonWriter.WriteStartObject();
                    foreach (DataTable table in dataSet!.Tables)
                    {
                        log.LogDebug($"Processing DataTable {table.TableName}");
                        WriteTable(jsonWriter, table, true);
                    }
                    jsonWriter.WriteEndObject();
                }
                jsonString = Encoding.UTF8.GetString(stream.ToArray());
            }

            log.LogDebug($"Json string length {jsonString.Length}{Environment.NewLine}Json:{Environment.NewLine}{jsonString}");

            using (var stream = new FileStream(fileConnection.Database, FileMode.Create, FileAccess.Write))
            using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
            {
                writer.Write(jsonString);
            }
        }
        catch (Exception ex)
        {
            log.LogError($"Failed to JsonDataSetWriter.{nameof(SaveToFile)}().  Error: {ex}");
            throw;
        }
    }

    private void SaveFolderAsDB(string? tableName, DataSet dataSet)
    {
        try
        {
            var tablesToWrite = dataSet!.Tables.Cast<DataTable>();
            if (!string.IsNullOrEmpty(tableName))
                tablesToWrite = tablesToWrite.Where(t => t.TableName == tableName);

            foreach (DataTable table in tablesToWrite)
            {
                var path = fileConnection.GetTablePath(table.TableName);
                log.LogDebug($"{GetType()}.{nameof(SaveFolderAsDB)}(). Saving file {path}");
                using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                using (var jsonWriter = new Utf8JsonWriter(fileStream, new JsonWriterOptions() { Indented = fileConnection.Formatted ?? false }))
                {
                    WriteTable(jsonWriter, table, false);
                }
            }
        }
        catch (Exception ex)
        {
            log.LogError($"Failed to JsonDataSetWriter.{nameof(SaveToFile)}().  Error: {ex}");
            throw;
        }

    }
}
