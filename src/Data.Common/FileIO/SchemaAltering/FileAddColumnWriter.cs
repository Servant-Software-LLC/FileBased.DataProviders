using Microsoft.Extensions.Logging;
using SqlBuildingBlocks;

namespace Data.Common.FileIO.SchemaAltering;

public abstract class FileAddColumnWriter : FileWriter
{
    private ILogger<FileAddColumnWriter> log => fileConnection.LoggerServices.CreateLogger<FileAddColumnWriter>();
    private readonly FileAddColumn fileAddColumn;

    public FileAddColumnWriter(FileAddColumn fileAddColumn, IFileConnection FileConnection, IFileCommand FileCommand)
        : base(FileConnection, FileCommand, fileAddColumn)
    {
        this.fileAddColumn = fileAddColumn ?? throw new ArgumentNullException(nameof(fileAddColumn));
    }

    public override int Execute()
    {
        log.LogDebug($"{nameof(FileAddColumnWriter)}.{nameof(Execute)}() called.  IsTransactedLater = {IsTransactedLater}");

        //TODO: ALTER TABLE ADD COLUMN is not supported within transactions at the moment.

        try
        {

            // As we have modified the File file so we don't need to update the tables
            _rwLock.EnterWriteLock();
            fileReader.StopWatching();

            var dataTable = fileReader.ReadFile(fileAddColumn, null, false);

            foreach (var columnDef in fileAddColumn.Columns)
            {
                if (dataTable.Columns.Contains(columnDef.ColumnName))
                    throw new InvalidOperationException($"The column {columnDef.ColumnName} already exists in the table {fileAddColumn.Tables.First()}. Statement = {fileAddColumn}");

                //Add the new column to the DataTable
                var dataType = fileConnection.DataTypeAlwaysString ? typeof(string) : DataType.ToSystemType(columnDef.DataType.Name) ?? typeof(string);
                var newColumn = new DataColumn(columnDef.ColumnName)
                {
                    DataType = dataType
                };

                dataTable.Columns.Add(newColumn);

                //TODO: Add support for setting the default value for the new column
                //foreach (DataRow dataRow in dataTable.Rows)
                //{
                //    dataRow[newColumn] = fileAddColumn.DefaultValue;
                //}
            }

        }
        finally
        {
            Save();

            _rwLock.ExitWriteLock();
            fileReader.StartWatching();
        }

        return -1;
    }
}
