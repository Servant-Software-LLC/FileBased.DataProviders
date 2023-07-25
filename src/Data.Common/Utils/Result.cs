using Data.Common.FileIO.Write;
using Microsoft.Extensions.Logging;
using SqlBuildingBlocks.Interfaces;
using SqlBuildingBlocks.LogicalEntities;
using SqlBuildingBlocks.QueryProcessing;
using SqlBuildingBlocks.Visitors;

namespace Data.Common.Utils;

internal class Result
{
    private readonly ILogger log;

    public DataTable WorkingResultSet { get; }
    public FileEnumerator FileEnumerator { get; }

    public object?[]? CurrentDataRow { get; private set; }
    public string Statement { get; }

    public Result(FileStatement fileStatement, FileReader fileReader, Func<FileStatement, FileWriter> createWriter, 
                  Result previousWriteResult, TransactionScopedRows transactionScopedRows, ILogger log)
    {
        Statement = fileStatement.Statement;
        this.log = log;

        //If SELECT statement
        if (fileStatement is FileSelect fileSelect)
        {
            //Normal SELECT query with a FROM <table> clause
            if (fileStatement.FromTable != null)
            {
                log.LogDebug("Normal SELECT query with a FROM <table> clause");
                WorkingResultSet = fileReader.ReadFile(fileStatement, transactionScopedRows, true);
            }
            else //SELECT query with no FROM clause
            {
                log.LogDebug("SELECT query with no FROM clause");
                WorkingResultSet = GetSingleRowTable(fileSelect, previousWriteResult);
            }

            if (WorkingResultSet == null)
                throw new ArgumentNullException(nameof(WorkingResultSet));

            log.LogDebug("Determine filter if any.");
            var filter = fileStatement!.Filter;
            if (filter != null)
            {
                //Check if any unsupported functions were provided.
                FunctionsEncounteredVisitor encounteredFunctions = new(new HashSet<string> { BuiltinFunction.RowCountFuncName, BuiltinFunction.LastInsertIdFuncName });
                filter.Accept(encounteredFunctions);
                if (encounteredFunctions.Unspecified.Count > 0)
                {
                    var invalidFunctionNames = string.Join(", ", encounteredFunctions.Unspecified);
                    throw new ArgumentException($"The following unsupported function names were encountered. {invalidFunctionNames}");
                }

                if (previousWriteResult != null && encounteredFunctions.Specified.Count > 0)
                    throw new ArgumentNullException(nameof(previousWriteResult), $"Cannot evaluate WHERE clause {filter} because it contains a built-in function that depends on a previous SQL statement being either an INSERT, UPDATE or DELETE statement.");

                if (previousWriteResult != null)
                {
                    log.LogDebug($"Resolving built-in functions.  Previous write result statement: {previousWriteResult.Statement}");

                    ResolveBuiltinFunctionsVisitor resolveBuiltinFunctionsVisitor = new(previousWriteResult);
                    filter.Accept(resolveBuiltinFunctionsVisitor);
                }
            }

            FileEnumerator = new FileEnumerator(fileStatement.Columns, WorkingResultSet, filter, log);

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

    /// <summary>
    /// This method is only called with a SELECT query with no FROM clause
    /// </summary>
    /// <param name="fileSelect"></param>
    /// <param name="previousWriteResult"></param>
    /// <returns></returns>
    private DataTable GetSingleRowTable(FileSelect fileSelect, Result previousWriteResult)
    {
        var dataTable = new DataTable();

        //Must iterate twice through the columns, so that in-between, we can call DataTable.NewRow() (i.e. the columns must already be defined in the DataTable before NewRow() is called)
        int unnamedColumnIndex = 1;
        foreach (ISqlColumn column in fileSelect.Columns)
        {
            switch(column)
            {
                case SqlAggregate:
                    throw new Exception($"An aggregate function cannot be used in a SELECT statement with no FROM clause.");

                case SqlParameterColumn:
                    throw new Exception($"A {nameof(SqlParameterColumn)} must be converted into a {nameof(SqlLiteralValueColumn)} prior to calling {nameof(QueryEngine.Query)} on the {typeof(QueryEngine)}");

                case SqlFunctionColumn functionColumn:

                    var function = functionColumn.Function;

                    //TODO: Temporary workaround.  Eventually, the QueryEngine should be taking on all the logic of this Result class
                    if (function.CalculateColumnValue == null)
                    {
                        function.CalculateColumnValue = () => BuiltinFunction.EvaluateFunction(function.FunctionName, previousWriteResult);
                    }

                    if (function.CalculateColumnValue == null)
                        throw new Exception($"A {nameof(SqlFunctionColumn)} must be converted into a {nameof(SqlLiteralValueColumn)} prior to calling {nameof(QueryEngine.Query)} on the {typeof(QueryEngine)} if the referenced {nameof(SqlFunction)} does not have the {nameof(SqlFunction.CalculateColumnValue)} property set.");

                    if (string.IsNullOrEmpty(functionColumn.ColumnAlias))
                    {
                        functionColumn.ColumnAlias = $"Column{unnamedColumnIndex++}";
                    }

                    dataTable.Columns.Add(functionColumn.ColumnAlias);
                    break;

                case ISqlColumnWithAlias columnWithAlias:
                    if (string.IsNullOrEmpty(columnWithAlias.ColumnAlias))
                    {
                        columnWithAlias.ColumnAlias = $"Column{unnamedColumnIndex++}";
                    }

                    dataTable.Columns.Add(columnWithAlias.ColumnAlias);
                    break;

                default:
                    throw new Exception($"The column {column} of type {column.GetType()} cannot be used (or at least needs support) in a SELECT statement with no FROM clause.");
            }

            
        }

        var newRow = dataTable.NewRow();
        foreach (ISqlColumnWithAlias column in fileSelect.Columns)
        {
            object value = null;
            switch (column)
            {
                case SqlFunctionColumn functionColumn:
                    //At this point, it has been guarded that functionColumn.Function.CalculateColumnValue is not null.
                    value = functionColumn.Function.CalculateColumnValue();
                    break;

                case SqlLiteralValueColumn literalValueColumn:
                    value = literalValueColumn.Value.Value;
                    break;

                default:
                    throw new Exception($"Unable to determine value in {nameof(GetSingleRowTable)} method for column of type {column.GetType()}");
            }

            if (value != null)
            {
                newRow[column.ColumnAlias] = value;
            }

        }
        dataTable.Rows.Add(newRow);

        return dataTable;
    }
}
