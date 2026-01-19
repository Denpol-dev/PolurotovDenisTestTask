namespace TR.Connectors.Api.Entities;

public sealed class UserProperty
{
    public string Name { get; }
    public string Value { get; }

    public UserProperty(string name, string value)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("User property name cannot be empty.", nameof(name));

        Name = name;
        Value = value ?? string.Empty;
    }
}