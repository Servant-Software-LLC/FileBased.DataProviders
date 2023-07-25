using Microsoft.Extensions.Logging;
using SqlBuildingBlocks.Interfaces;
using SqlBuildingBlocks.LogicalEntities;

namespace Data.Common.FileIO.Read;

internal class FileEnumerator : IEnumerator<object?[]>
{
    private readonly ILogger log;
    private readonly DataView workingDataView;  // We cannot use the DefaultView of the DataTable, because workingResultset may be a pointer directly to
                                                // one of the tables (i.e. not created from the columns/joins of the SELECT query) and many JsonDataReader/JsonEnumerator
                                                // may be instantiated with different filters on them.
    private object?[] currentRow = Array.Empty<object>();
    private bool endOfResultset;

    public FileEnumerator(IList<ISqlColumn> columns, DataTable workingResultset, SqlBinaryExpression? filter, ILogger log)
    {
        if (workingResultset == null) 
            throw new ArgumentNullException(nameof(workingResultset));

        this.log = log;

        log.LogDebug($"FileEnumerator creating. Filter? {filter != null}");
        workingDataView = new DataView(workingResultset);
        if (filter != null)
        {
            try
            {
                var sFilter = filter.ToString();
                log.LogDebug($"Filter: {sFilter}");
                workingDataView.RowFilter = sFilter;
            }
            catch(Exception ex)
            {
                log.LogError($"Filter could not be applied to DataView.  Error: {ex}");
            }
        }

        //If there is an asterisk indicating to show all columns.
        if (columns.Any(col => col.GetType() == typeof(SqlAllColumns))) 
        {
            foreach (DataColumn column in workingResultset.Columns)
            {
                Columns.Add(column.ColumnName);
            }
        }
        else
        {
            //TODO:  Temporary code again.  Eventually the QueryEngine will replace of all this specific code.
            //These columns need the column names (and not aliases) to get the values out of the workingResultset.  At
            //this point it is assumed that all columns are SqlColumnRef, because the FileBased provider has yet to 
            //support such things like SqlLiteralValueColumn instances.
            foreach (ISqlColumn sqlColumn in columns)
            {
                switch(sqlColumn)
                {
                    case SqlColumnRef sqlColumnRef:
                        Columns.Add(sqlColumnRef.ColumnName);
                        break;

                    default:
                        throw new ArgumentException($"The {nameof(FileEnumerator)} expected all columns provided to be of type {typeof(SqlColumnRef)}.  Type was {sqlColumn.GetType()}");
                }
            }

        }

        log.LogDebug($"FileEnumerator created.");
    }

    public object?[] Current => currentRow;
    object IEnumerator.Current => Current;
    public int CurrentIndex { get; private set; } = -1;
    public List<string> Columns { get; } = new List<string>();
    public int FieldCount => Columns.Count;

    public bool MoreRowsAvailable => workingDataView.Count > CurrentIndex;

    public bool HasRows => workingDataView.Count > 0;


    public bool MoveNext()
    {
        log.LogDebug($"FileEnumerator.MoveNext() called.  endofResultset = {endOfResultset}");

        if (endOfResultset)
            return false;

        CurrentIndex++;

        if (MoreRowsAvailable)
        {
            var row = workingDataView[CurrentIndex].Row;
            if (Columns?.FirstOrDefault()?.Trim() != "*")
            {
                currentRow = new object?[Columns!.Count];
                for (int i = 0; i < Columns?.Count; i++)
                {
                    currentRow[i] = row[Columns[i]];
                }
            }
            else
            {
                currentRow = row.ItemArray;
            }
            return true;
        }

        log.LogDebug($"End of resultset reached.");
        endOfResultset = true;
        return false;
    }

    public bool MoveNextInitial()
    {
        var res = MoveNext();
        Reset();
        return res;
    }

    public void Reset()
    {
        CurrentIndex = -1;
        //TableEnumerator.Reset();
    }

    public void Dispose()
    {
        //TableEnumerator?.Reset();
    }

    public string GetName(int i) => Columns[i];
    public int GetOrdinal(string name) => Columns.IndexOf(name);
    
    public Type GetType(int i)
    {
        var name = GetName(i);
        return workingDataView.Table!.Columns[name]!.DataType;
    }

}
