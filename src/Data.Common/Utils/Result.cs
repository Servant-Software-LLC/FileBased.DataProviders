namespace Data.Common.Utils;

internal class Result
{
    public DataTable WorkingResultSet { get; }
    public FileEnumerator FileEnumerator { get; }

    public object?[]? CurrentDataRow { get; private set; }
    public string Statement { get; }

    public Result(FileStatement fileStatement, FileReader fileReader, Func<FileStatement, FileWriter> createWriter)
    {
        Statement = fileStatement.Statement;

        //If SELECT statement
        if (fileStatement is FileSelect)
        {
            WorkingResultSet = fileReader.ReadFile(fileStatement, true);
            if (WorkingResultSet == null)
                throw new ArgumentNullException(nameof(WorkingResultSet));

            var filter = fileStatement!.Filter;

            FileEnumerator = new FileEnumerator(fileStatement.GetColumnNames(), WorkingResultSet, filter);

            RecordsAffected = -1;
            return;
        }

        //If INSERT, UPDATE, or DELETE statement
        var fileWriter = createWriter(fileStatement);

        RecordsAffected = fileWriter.Execute();
    }

    public int FieldCount => FileEnumerator == null ? 0 : FileEnumerator.FieldCount;

    public bool IsClosed => FileEnumerator == null || !FileEnumerator.MoreRowsAvailable;

    public bool HasRows => FileEnumerator != null && FileEnumerator.HasRows;

    public int RecordsAffected { get; private set; }

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
}
