namespace TR.Connectors.Api.Entities;

public sealed class UserToCreate
{
    public string Login { get; }
    public string HashPassword { get; }

    public IReadOnlyCollection<UserProperty> Properties { get; }

    public UserToCreate(string login, string hashPassword, IEnumerable<UserProperty>? properties = null)
    {
        if (string.IsNullOrWhiteSpace(login))
            throw new ArgumentException("User login cannot be empty.", nameof(login));

        if (string.IsNullOrWhiteSpace(hashPassword))
            throw new ArgumentException("User password hash cannot be empty.", nameof(hashPassword));

        Login = login;
        HashPassword = hashPassword;

        Properties = (properties ?? Array.Empty<UserProperty>()).ToList();
    }
}