using Data.Common.FileIO.Write;
using Microsoft.Extensions.Logging;

namespace Data.Common.Utils;

internal class Result
{
    private readonly ILogger log;

    public DataTable WorkingResultSet { get; }
    public FileEnumerator FileEnumerator { get; }

    public object?[]? CurrentDataRow { get; private set; }
    public string Statement { get; }

    public Result(FileStatement fileStatement, FileReader fileReader, Func<FileStatement, FileWriter> createWriter, 
                  Result previousWriteResult, Dictionary<string, List<DataRow>> transactionScopedRows, ILogger log)
    {
        Statement = fileStatement.Statement;
        this.log = log;

        //If SELECT statement
        if (fileStatement is FileSelect fileSelect)
        {
            //Normal SELECT query with a FROM <table> clause
            if (!string.IsNullOrEmpty(fileStatement.TableName))
            {
                log.LogDebug("Normal SELECT query with a FROM <table> clause");
                WorkingResultSet = fileReader.ReadFile(fileStatement, true, transactionScopedRows);
            }
            else //SELECT query with no FROM clause
            {
                log.LogDebug("SELECT query with no FROM clause");
                WorkingResultSet = GetSingleRowTable(fileSelect, previousWriteResult);
            }

            if (WorkingResultSet == null)
                throw new ArgumentNullException(nameof(WorkingResultSet));

            log.LogDebug("Determine filer if any.");
            var filter = fileStatement!.Filter;
            if (filter != null)
            {
                if (previousWriteResult is not null && filter.ContainsBuiltinFunction.HasValue && filter.ContainsBuiltinFunction.Value)
                    throw new ArgumentNullException(nameof(previousWriteResult), $"Cannot evaluate WHERE clause {filter} because it contains a built-in function that depends on a previous SQL statement being either an INSERT, UPDATE or DELETE statement.");

                log.LogDebug("Resolving built-in functions.");
                filter.ResolveFunctions(previousWriteResult);
            }

            FileEnumerator = new FileEnumerator(fileStatement.GetColumnNames(), WorkingResultSet, filter, log);

            RecordsAffected = -1;
            return;
        }

        //If INSERT, UPDATE, or DELETE statement
        var fileWriter = createWriter(fileStatement);

        RecordsAffected = fileWriter.Execute();

        if (fileWriter is FileInsertWriter fileInsertWriter)
        {
            LastInsertIdentity = fileInsertWriter.LastInsertIdentity;
            TransactionScopedRow = fileInsertWriter.TransactionScopedRow;
        }
    }

    public int FieldCount => FileEnumerator == null ? 0 : FileEnumerator.FieldCount;

    public bool IsClosed => FileEnumerator == null || !FileEnumerator.MoreRowsAvailable;

    public bool HasRows => FileEnumerator != null && FileEnumerator.HasRows;

    public int RecordsAffected { get; private set; }
    public object? LastInsertIdentity { get; private set; }
    public (string TableName, DataRow Row)? TransactionScopedRow { get; private set; }
    public bool Read()
    {
        if (FileEnumerator == null)
            return false;

        if (FileEnumerator.MoveNext())
        {
            CurrentDataRow = FileEnumerator.Current;
            return true;
        }

        return false;
    }

    private DataTable GetSingleRowTable(FileSelect fileSelect, Result previousWriteResult)
    {
        var dataTable = new DataTable();
        var columns = fileSelect.GetColumnNames();

        foreach (var column in columns)
        {
            dataTable.Columns.Add(column);
        }

        var newRow = dataTable.NewRow();
        foreach (var column in columns)
        {
            //NOTE: This isn't the proper way to do this, but limited on time at the moment in trying to get the EF Core Providers working.
            //      We're assuming the column name is the name of the function.
            var value = BuiltinFunction.EvaluateFunction(column, previousWriteResult);
            if (value != null)
            {
                newRow[column] = value;
            }
        }
        dataTable.Rows.Add(newRow);

        return dataTable;
    }
}
