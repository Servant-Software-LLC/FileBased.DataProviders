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

    public void WriteDataSet(FileReader fileReader)
    {
        if (fileConnection.DataSourceType == DataSourceType.Directory)
        {
            SaveFolderAsDB(fileQuery.FromTable.TableName, fileReader.DataSet);
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
            if (dataSet == null)
            {
                throw new Exception("Didn't expect that dataSet would be null in CsvDataSetWriter.");
            }

            var tablesToWrite = dataSet.Tables.Cast<VirtualDataTable>();
            if (!string.IsNullOrEmpty(tableName))
                tablesToWrite = tablesToWrite.Where(t => t.TableName == tableName);
            
            //Need a hard array, because we're about to start deleting from dataSet.Tables.
            tablesToWrite = tablesToWrite.ToArray();

            foreach (var table in tablesToWrite)
            {
                //For large data files, this is an expensive operation.
                var dataTable = table.ToDataTable();

                dataSet.Tables.Remove(table.TableName);

                //Free the virtual table, so that the file/Stream is freed.
                table.Rows = null;
                                

                log.LogDebug($"{GetType()}.{nameof(SaveFolderAsDB)}(). Saving file {fileConnection.Database}{fileConnection.DataSourceProvider.StorageIdentifier(table.TableName)}");
                using (var textWriter = fileConnection.DataSourceProvider.GetTextWriter(table.TableName))
                {
                    using (CsvWriter csv = new CsvWriter(textWriter, System.Globalization.CultureInfo.CurrentCulture))
                    {
                        // Write columns
                        foreach (DataColumn column in dataTable.Columns)
                            csv.WriteField(column.ColumnName);
                        csv.NextRecord();

                        // Write row values
                        foreach (DataRow row in dataTable.Rows)
                        {
                            for (var i = 0; i < table.Columns.Count; i++)
                            {
                                if (table.Columns[i].DataType == typeof(string) && row[i] == DBNull.Value)
                                {
                                    csv.WriteField("NULL");
                                }
                                else
                                {
                                    csv.WriteField(row[i]);
                                }
                            }
                            csv.NextRecord();
                        }
                    }
                }

                //Since the rows are already in memory, use them in a virtual table.
                table.Rows = dataTable.Rows.Cast<DataRow>().ToArray();
                dataSet.Tables.Add(table);
            }
        }
        catch (Exception ex)
        {
            log.LogError($"Failed to CsvDataSetWriter.{nameof(SaveFolderAsDB)}().  Error: {ex}");

            throw;
        }

    }
}