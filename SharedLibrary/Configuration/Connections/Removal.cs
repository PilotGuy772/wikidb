namespace SharedLibrary.Configuration.Connections;


/// <summary>
/// Represents an order to remove an element from a page.
/// </summary>
public struct Removal
{
    public RemovalType Type; // The way to identify the element to remove.
    public string Value; // The value to use to identify the element to remove.

    public Removal(RemovalType type, string value)
    {
        Type = type;
        Value = value;
    }
}

public enum RemovalType
{
    Xpath,
    Id,
    TagType
}