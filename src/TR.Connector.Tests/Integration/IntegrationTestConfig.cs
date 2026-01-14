namespace TR.Connector.Tests.Integration
{
    internal sealed class IntegrationTestConfig
    {
        public string ConnectionString { get; set; } = string.Empty;

        public string ExistingUserLogin { get; set; } = string.Empty;
        public string UserForPermissionsLogin { get; set; } = string.Empty;

        public ExpectedPermission ExpectedItRole { get; set; } = new();
        public ExpectedPermission ExpectedRight { get; set; } = new();

        public AssignPermissions AssignPermissions { get; set; } = new();

        public UserProperties UserProperties { get; set; } = new();

        public CreateUser CreateUser { get; set; } = new();
    }

    internal sealed class ExpectedPermission
    {
        public string Name { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
    }

    internal sealed class AssignPermissions
    {
        public string RoleId { get; set; } = string.Empty;
        public string RightId { get; set; } = string.Empty;
    }

    internal sealed class UserProperties
    {
        public UserPropertiesSnapshot Before { get; set; } = new();
        public UserPropertiesSnapshot After { get; set; } = new();
    }

    internal sealed class UserPropertiesSnapshot
    {
        public string FirstName { get; set; } = string.Empty;
        public string TelephoneNumber { get; set; } = string.Empty;
    }

    internal sealed class CreateUser
    {
        public string Password { get; set; } = string.Empty;
    }
}