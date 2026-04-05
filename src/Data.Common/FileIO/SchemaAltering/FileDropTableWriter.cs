using Microsoft.Extensions.Logging;
using SqlBuildingBlocks.POCOs;

namespace Data.Common.FileIO.SchemaAltering;

public abstract class FileDropTableWriter : FileWriter
{
    private ILogger<FileDropTableWriter> log => fileConnection.LoggerServices.CreateLogger<FileDropTableWriter>();

    public FileDropTableWriter(FileDropTable fileDropTable, IFileConnection fileConnection, IFileCommand fileCommand)
        : base(fileConnection, fileCommand, fileDropTable)
    {
    }

    public override int Execute()
    {
        log.LogDebug($"{nameof(FileDropTableWriter)}.{nameof(Execute)}() called.  IsTransactedLater = {IsTransactedLater}");

        var tableName = fileStatement.FromTable.TableName;

        // Check if the table exists before acquiring the write lock.
        // TableExists() acquires its own read lock, so must be called outside the write lock.
        if (!fileReader.TableExists(tableName))
            throw new TableNotFoundException($"Cannot drop table '{tableName}' because it does not exist.");

        try
        {
            readerWriterLock.EnterWriteLock();
            fileReader.StopWatching();

            // For folder-as-database, the table may not be in the DataSet yet
            // (TableExists only checks storage). Load it first if needed.
            if (fileConnection.FolderAsDatabase && !fileReader.DataSet.Tables.Contains(tableName))
            {
                // Table exists on disk but not loaded - we can skip MarkTableToUpdate
                // Just delete the storage directly.
            }
            else
            {
                // Free up the table's streams
                if (fileReader.DataSet.Tables.Contains(tableName))
                {
                    fileReader.MarkTableToUpdate(tableName);
                    fileReader.DataSet.Tables.Remove(tableName);
                }
            }

            // For folder-as-database, delete the underlying file
            if (fileConnection.FolderAsDatabase)
            {
                fileConnection.DataSourceProvider.DeleteStorage(tableName);
            }
        }
        finally
        {
            if (!fileConnection.FolderAsDatabase)
            {
                Save();
            }

            fileReader.StartWatching();
            readerWriterLock.ExitWriteLock();
        }

        return -1;
    }
}
