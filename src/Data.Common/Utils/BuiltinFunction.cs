namespace Data.Common.Utils;

static class BuiltinFunction
{
    internal static object? EvaluateFunction(string funcName, Result previousWriteResult)
    {
        if (string.Compare(funcName, "ROW_COUNT", true) == 0)
        {
            return previousWriteResult.RecordsAffected;
        }

        if (string.Compare(funcName, "LAST_INSERT_ID", true) == 0)
        {
            return previousWriteResult.LastInsertIdentity;
        }

        throw new InvalidOperationException($"Unknown function {funcName} in SQL statement.");
    }

}
