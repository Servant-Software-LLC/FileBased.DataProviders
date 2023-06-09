using Data.Common.Utils;

namespace Data.Common.FileStatements;

internal class Func
{
    public Func(string name)
    {
        Name = name;
    }

    public string Name { get; }
    public object Value { get; private set; }
    public void ResolveFunctionValue(Result previousWriteResult) => 
        Value = BuiltinFunction.EvaluateFunction(Name, previousWriteResult);
}
