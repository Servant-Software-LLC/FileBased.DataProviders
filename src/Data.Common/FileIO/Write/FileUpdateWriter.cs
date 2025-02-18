using SqlBuildingBlocks.LogicalEntities;
using SqlBuildingBlocks.POCOs;

namespace Data.Common.FileIO.Write;
public abstract class FileUpdateWriter : FileWriter
{
    private readonly FileUpdate fileUpdate;

    public FileUpdateWriter(FileUpdate fileStatement, IFileConnection FileConnection, IFileCommand FileCommand) 
        : base(FileConnection, FileCommand, fileStatement)
    {
        fileUpdate = fileStatement ?? throw new ArgumentNullException(nameof(fileStatement));
    }

    public override int Execute()
    {
        try
        {
            if (!IsTransaction)
            {
                // As we have modified the File file so we don't need to update the tables
                readerWriterLock.EnterWriteLock();
                fileReader.StopWatching();
            }

            var virtualDataTable = fileReader.ReadFile(fileUpdate, fileTransaction?.TransactionScopedRows);

            //Remember here, that the whole data table is going to reside in-memory at this point.
            var dataTable = virtualDataTable.ToDataTable();

            //Create a DataView to work with just for this operation
            var dataView = new DataView(dataTable);
            dataView.RowFilter = fileUpdate.Filter?.ToExpressionString();

            var rowsAffected = dataView.Count;

            //Don't update now if it is a transaction
            if (IsTransactedLater)
            {
                fileTransaction!.Writers.Add(this);
            }
            else
            {
                //This is not a transaction so update the data now
                foreach (DataRowView dataRow in dataView)
                {
                    foreach (SqlAssignment assignment in fileUpdate.Assignments)
                    {
                        var columnName = assignment.Column.ColumnName;
                        dataTable.Columns[columnName]!.ReadOnly = false;

                        if (assignment.Value == null)
                        {
                            var assignmentRight = assignment.Parameter != null ? $"{assignment.Parameter}({nameof(assignment.Parameter)})" : assignment.Function != null ? $"{assignment.Function}({nameof(assignment.Function)})" : "Unknown type";
                            throw new Exception($"Right side of the assigment did not supply a literal value.  Probably parameter or function wasn't resolved.  Right = {assignmentRight}");
                        }

                        dataRow[columnName] = assignment.Value.Value;
                    }
                }
            }

            //Save the results of the deletion back onto the virtual table, which will get saved in the Save() call below.
            virtualDataTable.Columns = dataTable.Columns;
            virtualDataTable.Rows = dataTable.Rows.Cast<DataRow>();

            Save();
            return rowsAffected;
        }
        finally
        {
            if (!IsTransaction)
            {
                readerWriterLock.ExitWriteLock();
                fileReader.StartWatching();
            }
        }

    }
}
