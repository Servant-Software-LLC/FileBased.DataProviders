using SqlBuildingBlocks.LogicalEntities;

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
                _rwLock.EnterWriteLock();
                fileReader.StopWatching();
            }

            var dataTable = fileReader.ReadFile(fileUpdate, fileTransaction?.TransactionScopedRows);

            //Create a DataView to work with just for this operation
            var dataView = new DataView(dataTable);
            dataView.RowFilter = fileUpdate.Filter?.ToExpressionString();

            var rowsAffected = dataView.Count;

            //don't update now if it is a transaction
            if (IsTransactedLater)
            {
                fileTransaction!.Writers.Add(this);
                return rowsAffected;
            }
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

            return rowsAffected;
        }
        finally
        {
            Save();

            if (!IsTransaction)
            {
                _rwLock.ExitWriteLock();
                fileReader.StartWatching();
            }
        }

    }
}
