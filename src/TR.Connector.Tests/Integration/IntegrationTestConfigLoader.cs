using Microsoft.Extensions.Configuration;

namespace TR.Connector.Tests.Integration
{
    internal static class IntegrationTestConfigLoader
    {
        private const string SectionName = "ConnectorIntegration";

        public static IntegrationTestConfig Load()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();

            var config = configuration.GetSection(SectionName).Get<IntegrationTestConfig>();

            if (config is null)
                throw new InvalidOperationException($"Missing '{SectionName}' section in appsettings.json.");

            Validate(config);
            return config;
        }

        private static void Validate(IntegrationTestConfig config)
        {
            if (string.IsNullOrWhiteSpace(config.ConnectionString))
                throw new InvalidOperationException("ConnectorIntegration:ConnectionString is required.");

            if (string.IsNullOrWhiteSpace(config.ExistingUserLogin))
                throw new InvalidOperationException("ConnectorIntegration:ExistingUserLogin is required.");

            if (string.IsNullOrWhiteSpace(config.UserForPermissionsLogin))
                throw new InvalidOperationException("ConnectorIntegration:UserForPermissionsLogin is required.");

            if (string.IsNullOrWhiteSpace(config.ExpectedItRole?.Name) || string.IsNullOrWhiteSpace(config.ExpectedItRole?.Id))
                throw new InvalidOperationException("ConnectorIntegration:ExpectedItRole (Name/Id) is required.");

            if (string.IsNullOrWhiteSpace(config.ExpectedRight?.Name) || string.IsNullOrWhiteSpace(config.ExpectedRight?.Id))
                throw new InvalidOperationException("ConnectorIntegration:ExpectedRight (Name/Id) is required.");

            if (string.IsNullOrWhiteSpace(config.AssignPermissions?.RoleId) || string.IsNullOrWhiteSpace(config.AssignPermissions?.RightId))
                throw new InvalidOperationException("ConnectorIntegration:AssignPermissions (RoleId/RightId) is required.");

            if (string.IsNullOrWhiteSpace(config.UserProperties?.Before?.FirstName) ||
                string.IsNullOrWhiteSpace(config.UserProperties?.Before?.TelephoneNumber) ||
                string.IsNullOrWhiteSpace(config.UserProperties?.After?.FirstName) ||
                string.IsNullOrWhiteSpace(config.UserProperties?.After?.TelephoneNumber))
                throw new InvalidOperationException("ConnectorIntegration:UserProperties (Before/After) is required.");

            if (string.IsNullOrWhiteSpace(config.CreateUser?.Password))
                throw new InvalidOperationException("ConnectorIntegration:CreateUser:Password is required.");
        }
    }
}