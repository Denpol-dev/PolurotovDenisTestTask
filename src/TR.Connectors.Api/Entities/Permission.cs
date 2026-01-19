namespace TR.Connectors.Api.Entities;

public sealed class Permission
{
    public string Id { get; }
    public string Name { get; }
    public string Description { get; }

    public Permission(string id, string name, string description)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Permission id cannot be empty.", nameof(id));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Permission name cannot be empty.", nameof(name));

        Id = id;
        Name = name;
        Description = description ?? string.Empty;
    }
}