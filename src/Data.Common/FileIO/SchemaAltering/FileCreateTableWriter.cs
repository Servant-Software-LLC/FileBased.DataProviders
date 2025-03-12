using Microsoft.Extensions.Logging;
using SqlBuildingBlocks.Grammars.SQLServer;
using SqlBuildingBlocks.LogicalEntities;
using SqlBuildingBlocks.POCOs;

namespace Data.Common.FileIO.SchemaAltering;

public abstract class FileCreateTableWriter : FileWriter
{
    private ILogger<FileCreateTableWriter> log => fileConnection.LoggerServices.CreateLogger<FileCreateTableWriter>();

    public FileCreateTableWriter(FileCreateTable fileStatement, IFileConnection fileConnection, IFileCommand fileCommand)
        : base(fileConnection, fileCommand, fileStatement)
    {
    }

    public override int Execute()
    {
        log.LogDebug($"{nameof(FileCreateTableWriter)}.{nameof(Execute)}() called.  IsTransactedLater = {IsTransactedLater}");

        //TODO: CREATE TABLE is not supported within transactions at the moment.

        var tableName = fileStatement.FromTable.TableName;

        //Create the new table (lock isn't required until we want to add the new table to the DataSet.
        DataTable newTable = new(tableName);

        if (fileStatement is not FileCreateTable fileCreateTableStatement)
            throw new Exception($"Expected {nameof(fileStatement)} to be a {nameof(FileCreateTable)}");

        foreach (SqlColumnDefinition columnDefinition in fileCreateTableStatement.Columns)
        {
            var dataType = fileConnection.DataTypeAlwaysString ? typeof(string) : DataType.ToSystemType(columnDefinition.DataType.Name) ?? typeof(string);
            DataColumn newColumn = new(columnDefinition.ColumnName)
            {
                DataType = dataType
            };

            newTable.Columns.Add(newColumn);
        }

        var columnsOnTable = fileReader.ColumnsOnTable(fileStatement.FromTable.TableName);

        bool tableWithNoColumns = false;
        if (columnsOnTable.HasValue)
        {
            //Only allow the table to be 'created' if there are no columns on the table.
            if (columnsOnTable.Value > 0)
                throw new InvalidOperationException($"The database already contains a table named {tableName}. Statement = {fileStatement}");

            tableWithNoColumns = true;
        }

        try
        {
            readerWriterLock.EnterWriteLock();
            //as we have modified the json file so we don't need to update the tables
            fileReader.StopWatching();

            if (tableWithNoColumns)
            {
                //Free up the table's streams.
                fileReader.MarkTableToUpdate(tableName);

                //Remove the table from the DataSet.
                fileReader.DataSet.Tables.Remove(tableName);
            }

            fileReader.DataSet.Tables.Add(new VirtualDataTable(newTable));
        }
        finally
        {
            Save();

            fileReader.StartWatching();
            readerWriterLock.ExitWriteLock();
        }

        return -1;
    }
}
