using Data.Common.DataSource;
using Microsoft.Extensions.Logging;
using SqlBuildingBlocks.POCOs;

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

    public void WriteDataSet(FileReader fileReader)
    {
        if (fileConnection.DataSourceType == DataSourceType.Directory)
        {
            SaveFolderAsDB(fileQuery.FromTable.TableName, fileReader.DataSet);
        }
        else
        {
            SaveToFile(fileReader);
        }
    }

    private void SaveToFile(FileReader fileReader)
    {
        try
        {
            log.LogDebug($"{GetType()}.{nameof(SaveToFile)}(). Saving file {fileConnection.Database}");
            using (var textWriter = fileConnection.DataSourceProvider.GetTextWriter(string.Empty))
            {
                WriteXml(fileReader.DataSet, textWriter, XmlWriteMode.WriteSchema);
            }
        }
        catch (Exception ex)
        {
            log.LogError($"Failed to XmlDataSetWriter.{nameof(SaveToFile)}().  Error: {ex}");
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

            foreach (var virtualDataTable in tablesToWrite)
            {
                log.LogDebug($"{GetType()}.{nameof(SaveFolderAsDB)}(). Saving file {fileConnection.Database}{fileConnection.DataSourceProvider.StorageIdentifier(virtualDataTable.TableName)}");
                using (var textWriter = fileConnection.DataSourceProvider.GetTextWriter(virtualDataTable.TableName))
                {
                    DataTable dataTable = virtualDataTable.ToDataTable();

                    dataTable.WriteXml(textWriter, XmlWriteMode.WriteSchema);
                }
            }
        }
        catch (Exception ex)
        {
            log.LogError($"Failed to XmlDataSetWriter.{nameof(SaveFolderAsDB)}().  Error: {ex}");
            throw;
        }
    }

    private void WriteXml(VirtualDataSet virtualDataSet, TextWriter writer, XmlWriteMode mode)
    {
        // Create a temporary DataSet
        DataSet dataSet = new();
        foreach (var table in virtualDataSet.Tables)
        {
            //Virtual tables are copied into real DataTable.  Can cause issues for very large table data.
            var dataTable = table.ToDataTable();
            dataTable.TableName = table.TableName;

            dataSet.Tables.Add(dataTable);
        }

        // Write the DataSet to the file, including the schema.
        dataSet.WriteXml(writer, XmlWriteMode.WriteSchema);
    }

}
