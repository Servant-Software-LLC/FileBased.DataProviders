namespace Data.Json.Utils.ConnectionString;

/// <summary>
/// General-purpose attribute to put an alias on entities.  Allows for a space, which entity naming does not.
/// </summary>
[AttributeUsage(
    AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Method | AttributeTargets.Class,
    AllowMultiple = false)]
public class AliasAttribute : Attribute
{
    public AliasAttribute(string name) => Name = name;

    public string Name { get; }
}
