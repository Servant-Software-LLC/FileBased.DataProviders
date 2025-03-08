﻿using Data.Common.DataSource;
using Microsoft.Extensions.Logging;
using SqlBuildingBlocks.POCOs;
using System.Diagnostics;
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
        this.fileStatement = fileStatement;
    }

    public void WriteDataSet(VirtualDataSet virtualDataSet)
    {
        if (fileConnection.DataSourceType == DataSourceType.Directory)
        {
            SaveFolderAsDB(fileStatement.FromTable.TableName, virtualDataSet);
        }
        else
        {
            SaveFileAsDB(virtualDataSet);
        }
    }

    /// <summary>
    /// Provides an entry point for a derived class to add a comment
    /// </summary>
    /// <param name="jsonWriter"></param>
    protected virtual void WriteCommentValue(Utf8JsonWriter jsonWriter, DataColumn column) { }

    private void WriteTable(Utf8JsonWriter jsonWriter, VirtualDataTable table, bool writeTableName)
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
                    case "Float":
                        jsonWriter.WriteNumber(column.ColumnName, (float)row[column]);
                        break;
                    case "Double":
                        jsonWriter.WriteNumber(column.ColumnName, (double)row[column]);
                        break;
                    case "Decimal":
                        jsonWriter.WriteNumber(column.ColumnName, (decimal)row[column]);
                        break;
                    case "String":
                        try
                        {
                            jsonWriter.WriteString(column.ColumnName, row[column].ToString().AsSpan());
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.ToString());
                        }
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

    private void SaveFileAsDB(VirtualDataSet virtualDataSet)
    {
        try
        {
            log.LogDebug($"{GetType()}.{nameof(SaveFileAsDB)}(). Saving file {fileConnection.Database}");

            string jsonString;
            using (var stream = new MemoryStream())
            {
                using (var jsonWriter = new Utf8JsonWriter(stream, new JsonWriterOptions() { Indented = fileConnection.Formatted ?? false }))
                {
                    jsonWriter.WriteStartObject();
                    foreach (var table in virtualDataSet!.Tables)
                    {
                        log.LogDebug($"Processing DataTable {table.TableName}");
                        WriteTable(jsonWriter, table, true);
                    }
                    jsonWriter.WriteEndObject();
                }
                jsonString = Encoding.UTF8.GetString(stream.ToArray());
            }

            log.LogDebug($"Json string length {jsonString.Length}{Environment.NewLine}Json:{Environment.NewLine}{jsonString}");

            using (var textWriter = fileConnection.DataSourceProvider.GetTextWriter(string.Empty))
            {
                textWriter.Write(jsonString);
            }
        }
        catch (Exception ex)
        {
            log.LogError($"Failed to JsonDataSetWriter.{nameof(SaveFileAsDB)}().  Error: {ex}");
            throw;
        }
    }

    private void SaveFolderAsDB(string tableName, VirtualDataSet dataSet)
    {
        try
        {
            var tablesToWrite = dataSet!.Tables.Cast<VirtualDataTable>();
            if (!string.IsNullOrEmpty(tableName))
                tablesToWrite = tablesToWrite.Where(t => t.TableName == tableName);

            foreach (VirtualDataTable table in tablesToWrite)
            {
                log.LogDebug($"{GetType()}.{nameof(SaveFolderAsDB)}(). Saving file {fileConnection.Database}{fileConnection.DataSourceProvider.StorageIdentifier(table.TableName)}");

                string jsonString;
                using (var stream = new MemoryStream())
                {
                    using (var jsonWriter = new Utf8JsonWriter(stream, new JsonWriterOptions() { Indented = fileConnection.Formatted ?? false }))
                    {
                        WriteTable(jsonWriter, table, false);
                    }

                    jsonString = Encoding.UTF8.GetString(stream.ToArray());
                }

                using (var textWriter = fileConnection.DataSourceProvider.GetTextWriter(table.TableName))
                {
                    textWriter.Write(jsonString);
                }
            }
        }
        catch (Exception ex)
        {
            log.LogError($"Failed to JsonDataSetWriter.{nameof(SaveFileAsDB)}().  Error: {ex}");
            throw;
        }

    }
}
