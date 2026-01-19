namespace TR.Connectors.Api.Entities;

public sealed class Property
{
    public string Name { get; }
    public string Description { get; }

    public Property(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Property name cannot be empty.", nameof(name));

        Name = name;
        Description = description ?? string.Empty;
    }
}