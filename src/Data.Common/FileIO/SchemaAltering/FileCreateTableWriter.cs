using Microsoft.Extensions.Logging;
using SqlBuildingBlocks.Grammars.SQLServer;
using SqlBuildingBlocks.LogicalEntities;

namespace Data.Common.FileIO.SchemaAltering;

public abstract class FileCreateTableWriter : FileWriter
{
    private ILogger<FileCreateTableWriter> log => fileConnection.LoggerServices.CreateLogger<FileCreateTableWriter>();
    private readonly FileCreateTable fileStatement;

    public FileCreateTableWriter(FileCreateTable fileStatement, IFileConnection fileConnection, IFileCommand fileCommand)
        : base(fileConnection, fileCommand, fileStatement)
    {
        this.fileStatement = fileStatement ?? throw new ArgumentNullException(nameof(fileStatement));
    }

    public override int Execute()
    {
        log.LogDebug($"{nameof(FileCreateTableWriter)}.{nameof(Execute)}() called.  IsTransactedLater = {IsTransactedLater}");

        //TODO: CREATE TABLE is not supported within transactions at the moment.


        var tableName = fileStatement.FromTable.TableName;

        //Create the new table (lock isn't required until we want to add the new table to the DataSet.
        DataTable newTable = new(tableName);

        foreach (SqlColumnDefinition columnDefinition in fileStatement.Columns)
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
                //Remove the table from the DataSet.
                fileReader.DataSet.Tables.Remove(tableName);
            }

            fileReader.DataSet.Tables.Add(newTable);
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
