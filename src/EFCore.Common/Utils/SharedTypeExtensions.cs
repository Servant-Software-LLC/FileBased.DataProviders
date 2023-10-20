using System.Reflection;

namespace EFCore.Common.Utils;

public static class SharedTypeExtensions
{
    /// <summary>
    /// Gets the runtime method with the specified name and parameters from the given type.
    /// </summary>
    /// <param name="type">The type to search for the method.</param>
    /// <param name="name">The name of the method to find.</param>
    /// <param name="parameters">The parameter types of the method to find.</param>
    /// <returns>The found method.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the method with the specified name and parameters is not found.</exception>
    public static MethodInfo GetRequiredRuntimeMethod(this Type type, string name, params Type[] parameters)
        => type.GetTypeInfo().GetRuntimeMethod(name, parameters)
            ?? throw new InvalidOperationException($"Could not find method '{name}' on type '{type}'");

}
