using CsvHelper;
using Data.Common.DataSource;
using Microsoft.Extensions.Logging;
using SqlBuildingBlocks.POCOs;

namespace Data.Csv.CsvIO;

internal class CsvDataSetWriter : IDataSetWriter
{
    private ILogger<CsvDataSetWriter> log => ((IFileConnection)fileConnection).LoggerServices.CreateLogger<CsvDataSetWriter>();
    private readonly IFileConnection fileConnection;
    private readonly FileStatement fileQuery;

    public CsvDataSetWriter(IFileConnection fileConnection, FileStatement fileQuery)
    {
        this.fileConnection = fileConnection;
        this.fileQuery = fileQuery;
    }

    public void WriteDataSet(VirtualDataSet dataSet)
    {
        if (fileConnection.DataSourceType == DataSourceType.Directory)
        {
            SaveFolderAsDB(fileQuery.FromTable.TableName, dataSet);
        }
        else
        {
            throw new InvalidOperationException("File as database is not supported in Csv Provider");
        }
    }

    private void SaveFolderAsDB(string tableName, VirtualDataSet dataSet)
    {
        try
        {
            var tablesToWrite = dataSet!.Tables.Cast<VirtualDataTable>();
            if (!string.IsNullOrEmpty(tableName))
                tablesToWrite = tablesToWrite.Where(t => t.TableName == tableName);

            foreach (var table in tablesToWrite)
            {
                log.LogDebug($"{GetType()}.{nameof(SaveFolderAsDB)}(). Saving file {fileConnection.Database}{fileConnection.DataSourceProvider.StorageIdentifier(table.TableName)}");
                using (var textWriter = fileConnection.DataSourceProvider.GetTextWriter(table.TableName))
                {
                    using (CsvWriter csv = new CsvWriter(textWriter, System.Globalization.CultureInfo.CurrentCulture))
                    {
                        // Write columns
                        foreach (DataColumn column in table.Columns)
                            csv.WriteField(column.ColumnName);
                        csv.NextRecord();

                        // Write row values
                        foreach (DataRow row in table.Rows)
                        {
                            for (var i = 0; i < table.Columns.Count; i++)
                                csv.WriteField(row[i]);
                            csv.NextRecord();
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            log.LogError($"Failed to CsvDataSetWriter.{nameof(SaveFolderAsDB)}().  Error: {ex}");

            throw;
        }

    }
}