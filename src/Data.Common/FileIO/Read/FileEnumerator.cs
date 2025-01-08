using Microsoft.Extensions.Logging;

namespace Data.Common.FileIO.Read;

internal class FileEnumerator : IEnumerator<object[]>
{
    private readonly ILogger log;
    private readonly DataTable resultset;
    private object[] currentRow = Array.Empty<object>();
    private bool endOfResultset;

    public FileEnumerator(DataTable workingResultset, ILogger log)
    {
        if (workingResultset == null) 
            throw new ArgumentNullException(nameof(workingResultset));

        this.log = log;

        resultset = workingResultset;
    }

    public object[] Current => currentRow;
    object IEnumerator.Current => Current;
    public int CurrentIndex { get; private set; } = -1;
    public DataColumnCollection Columns => resultset.Columns;
    public int FieldCount => resultset.Columns.Count;

    public bool MoreRowsAvailable => resultset.Rows.Count > CurrentIndex;

    public bool HasRows => resultset.Rows.Count > 0;


    public bool MoveNext()
    {
        log.LogDebug($"FileEnumerator.MoveNext() called.  endofResultset = {endOfResultset}");

        if (endOfResultset)
            return false;

        CurrentIndex++;

        if (MoreRowsAvailable)
        {
            currentRow = resultset.Rows[CurrentIndex].ItemArray;
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

    public string GetName(int i) => resultset.Columns[i].ColumnName;
    public int GetOrdinal(string name) => resultset.Columns.IndexOf(name);

    public Type GetType(int i) => resultset.Columns[i].DataType;

}
