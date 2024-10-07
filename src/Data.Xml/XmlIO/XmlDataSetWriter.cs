using Data.Common.DataSource;
using Microsoft.Extensions.Logging;

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
        if (fileConnection.DataSourceType == DataSourceType.Directory)
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
            using (var textWriter = fileConnection.DataSourceProvider.GetTextWriter(string.Empty))
            {
                dataSet.WriteXml(textWriter, XmlWriteMode.WriteSchema);
            }
        }
        catch (Exception ex)
        {
            log.LogError($"Failed to XmlDataSetWriter.{nameof(SaveToFile)}().  Error: {ex}");
            throw;
        }
    }

    private void SaveFolderAsDB(string tableName, DataSet dataSet)
    {
        try
        {
            var tablesToWrite = dataSet!.Tables.Cast<DataTable>();
            if (!string.IsNullOrEmpty(tableName))
                tablesToWrite = tablesToWrite.Where(t => t.TableName == tableName);

            foreach (DataTable table in tablesToWrite)
            {
                log.LogDebug($"{GetType()}.{nameof(SaveFolderAsDB)}(). Saving file {fileConnection.Database}{fileConnection.DataSourceProvider.StorageIdentifier(table.TableName)}");
                using (var textWriter = fileConnection.DataSourceProvider.GetTextWriter(table.TableName))
                {
                    table.WriteXml(textWriter, XmlWriteMode.WriteSchema);
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
