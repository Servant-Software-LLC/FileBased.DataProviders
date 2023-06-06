namespace Data.Common.FileStatements;

internal class Field
{
    public Field(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public override string ToString() => Name;
}
