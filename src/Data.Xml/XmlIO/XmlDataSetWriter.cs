﻿using Microsoft.Extensions.Logging;

namespace Data.Xml.XmlIO;

internal class XmlDataSetWriter : IDataSetWriter
{
    private ILogger<XmlDataSetWriter> log => ((IFileConnection)fileConnection).LoggerServices.CreateLogger<XmlDataSetWriter>();
    private readonly IFileConnection fileConnection;
    private readonly FileStatement fileQuery;

    public XmlDataSetWriter(IFileConnection fileConnection, FileStatement fileQuery)
    {
        this.fileConnection = fileConnection;
        this.fileQuery = fileQuery;
    }

    public void WriteDataSet(DataSet dataSet)
    {
        if (fileConnection.PathType == PathType.Directory)
        {
            SaveFolderAsDB(fileQuery.FromTable.TableName, dataSet);
        }
        else
        {
            SaveToFile(dataSet);
        }
    }

    private void SaveToFile(DataSet dataSet)
    {
        try
        {
            log.LogDebug($"{GetType()}.{nameof(SaveToFile)}(). Saving file {fileConnection.Database}");
            using (var fileStream = new FileStream(fileConnection.Database, FileMode.Create, FileAccess.Write))
            {
                dataSet.WriteXml(fileStream, XmlWriteMode.WriteSchema);
            }
        }
        catch (Exception ex)
        {
            log.LogError($"Failed to XmlDataSetWriter.{nameof(SaveToFile)}().  Error: {ex}");
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
                {
                    table.WriteXml(fileStream, XmlWriteMode.WriteSchema);
                }
            }
        }
        catch (Exception ex)
        {
            log.LogError($"Failed to XmlDataSetWriter.{nameof(SaveFolderAsDB)}().  Error: {ex}");
            throw;
        }
    }
}
