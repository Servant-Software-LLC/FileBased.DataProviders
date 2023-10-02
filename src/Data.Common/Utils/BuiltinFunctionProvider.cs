using SqlBuildingBlocks.Interfaces;
using SqlBuildingBlocks.LogicalEntities;

namespace Data.Common.Utils;

class BuiltinFunctionProvider : IFunctionProvider
{
    internal const string RowCountFuncName = "ROW_COUNT";
    internal const string LastInsertIdFuncName = "LAST_INSERT_ID";

    private readonly Result previousWriteResult;

    public BuiltinFunctionProvider(Result previousWriteResult)
    {
        this.previousWriteResult = previousWriteResult ?? throw new ArgumentNullException(nameof(previousWriteResult));    
    }

    public Type GetDataType(SqlFunction sqlFunction)
    {
        var funcName = sqlFunction.FunctionName;
        if (string.Compare(funcName, RowCountFuncName, true) == 0)
        {
            return previousWriteResult.RecordsAffected.GetType();
        }

        if (string.Compare(funcName, LastInsertIdFuncName, true) == 0)
        {
            return previousWriteResult.LastInsertIdentity.GetType();
        }

        throw new InvalidOperationException($"Unknown function {funcName} in SQL statement.");
    }

    public Func<object> GetDataValue(SqlFunction sqlFunction)
    {
        var funcName = sqlFunction.FunctionName;
        if (string.Compare(funcName, RowCountFuncName, true) == 0)
        {
            return () => previousWriteResult.RecordsAffected;
        }

        if (string.Compare(funcName, LastInsertIdFuncName, true) == 0)
        {
            return () => previousWriteResult.LastInsertIdentity;
        }

        throw new InvalidOperationException($"Unknown function {funcName} in SQL statement.");
    }

}
