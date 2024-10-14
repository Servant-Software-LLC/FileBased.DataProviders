using Microsoft.Extensions.Logging;

namespace Data.Common.FileIO.SchemaAltering;

public abstract class FileDropColumnWriter : FileWriter
{
    private ILogger<FileDropColumnWriter> log => fileConnection.LoggerServices.CreateLogger<FileDropColumnWriter>();
    private readonly FileDropColumn fileDropColumn;

    public FileDropColumnWriter(FileDropColumn fileDropColumn, IFileConnection FileConnection, IFileCommand FileCommand)
        : base(FileConnection, FileCommand, fileDropColumn)
    {
        this.fileDropColumn = fileDropColumn ?? throw new ArgumentNullException(nameof(fileDropColumn));
    }

    public override int Execute()
    {
        log.LogDebug($"{nameof(FileAddColumnWriter)}.{nameof(Execute)}() called.  IsTransactedLater = {IsTransactedLater}");

        //TODO: ALTER TABLE DROP COLUMN is not supported within transactions at the moment.

        try
        {

            // As we have modified the File file so we don't need to update the tables
            readerWriterLock.EnterWriteLock();
            fileReader.StopWatching();

            var dataTable = fileReader.ReadFile(fileDropColumn, null, false);

            foreach (var columnName in fileDropColumn.Columns)
            {
                if (!dataTable.Columns.Contains(columnName))
                    throw new InvalidOperationException($"The column {columnName} does not exist in the table {fileDropColumn.Tables.First()}. Statement = {fileDropColumn}");

                //Remove the column from the DataTable
                dataTable.Columns.Remove(columnName);
            }

        }
        finally
        {
            Save();

            readerWriterLock.ExitWriteLock();
            fileReader.StartWatching();
        }

        return -1;
    }

}
