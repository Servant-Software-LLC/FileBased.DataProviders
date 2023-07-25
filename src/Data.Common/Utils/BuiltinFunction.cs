namespace Data.Common.Utils;

static class BuiltinFunction
{
    internal const string RowCountFuncName = "ROW_COUNT";
    internal const string LastInsertIdFuncName = "LAST_INSERT_ID";

    internal static object? EvaluateFunction(string funcName, Result previousWriteResult)
    {
        if (string.Compare(funcName, RowCountFuncName, true) == 0)
        {
            return previousWriteResult.RecordsAffected;
        }

        if (string.Compare(funcName, LastInsertIdFuncName, true) == 0)
        {
            return previousWriteResult.LastInsertIdentity;
        }

        throw new InvalidOperationException($"Unknown function {funcName} in SQL statement.");
    }

}
