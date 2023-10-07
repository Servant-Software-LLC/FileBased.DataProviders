using System.Reflection;

namespace EFCore.Common.Utils;

public static class SharedTypeExtensions
{
    public static MethodInfo GetRequiredRuntimeMethod(this Type type, string name, params Type[] parameters)
        => type.GetTypeInfo().GetRuntimeMethod(name, parameters)
            ?? throw new InvalidOperationException($"Could not find method '{name}' on type '{type}'");

}
