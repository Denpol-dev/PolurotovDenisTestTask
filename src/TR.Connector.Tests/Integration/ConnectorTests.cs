using TR.Connectors.Api.Entities;
using TR.Connectors.Api.Interfaces;

namespace TR.Connector.Tests.Integration
{
    [Trait("Category", "Integration")]
    public class ConnectorTests
    {
        private readonly IConnector _connector;
        private readonly IntegrationTestConfig _config;

        public ConnectorTests()
        {
            _config = IntegrationTestConfigLoader.Load();

            _connector = new Connector
            {
                Logger = new ConsoleLogger()
            };

            _connector.StartUp(_config.ConnectionString);
        }

        [Fact]
        public void GetAllPermissions_Ok()
        {
            var permissions = _connector.GetAllPermissions();
            Assert.NotNull(permissions);

            var itRole = permissions.FirstOrDefault(p => p.Name == _config.ExpectedItRole.Name);
            Assert.NotNull(itRole);
            Assert.Equal(_config.ExpectedItRole.Id, itRole!.Id);

            var right = permissions.FirstOrDefault(p => p.Name == _config.ExpectedRight.Name);
            Assert.NotNull(right);
            Assert.Equal(_config.ExpectedRight.Id, right!.Id);
        }

        [Fact]
        public void GetUserPermissions_Ok()
        {
            var permissions = _connector.GetUserPermissions(_config.ExistingUserLogin).ToList();

            Assert.NotNull(permissions);
            Assert.Contains(permissions, p => p.Contains("ItRole"));
            Assert.Contains(permissions, p => p.Contains("RequestRight"));
        }

        [Fact]
        public void Add_Drop_Permissions_Ok()
        {
            var login = _config.UserForPermissionsLogin;
            var userRole = _config.AssignPermissions.RoleId;
            var userRight = _config.AssignPermissions.RightId;

            _connector.AddUserPermissions(login, new List<string> { userRole, userRight });

            var userPermissions = _connector.GetUserPermissions(login).ToList();
            Assert.Contains(userPermissions, p => p.Contains(userRole));
            Assert.Contains(userPermissions, p => p.Contains(userRight));

            _connector.RemoveUserPermissions(login, new List<string> { userRole, userRight });

            userPermissions = _connector.GetUserPermissions(login).ToList();
            Assert.DoesNotContain(userPermissions, p => p.Contains(userRole));
            Assert.DoesNotContain(userPermissions, p => p.Contains(userRight));
        }

        [Fact]
        public void GetAllProperties_Ok()
        {
            var allProperties = _connector.GetAllProperties();

            Assert.NotNull(allProperties);
            Assert.Contains(allProperties, p => p.Name.Contains("isLead"));
        }

        [Fact]
        public void Get_UpdateUserProperties_Ok()
        {
            var login = _config.ExistingUserLogin;

            var userProperties = _connector.GetUserProperties(login).ToList();
            Assert.NotNull(userProperties);

            Assert.Equal(_config.UserProperties.Before.FirstName, userProperties.First(p => p.Name == "firstName").Value);
            Assert.Equal(_config.UserProperties.Before.TelephoneNumber, userProperties.First(p => p.Name == "telephoneNumber").Value);

            var userProps = new List<UserProperty>
            {
                new UserProperty("firstName", _config.UserProperties.After.FirstName),
                new UserProperty("telephoneNumber", _config.UserProperties.After.TelephoneNumber),
            };

            _connector.UpdateUserProperties(userProps, login);

            userProperties = _connector.GetUserProperties(login).ToList();
            Assert.NotNull(userProperties);

            Assert.Equal(_config.UserProperties.After.FirstName, userProperties.First(p => p.Name == "firstName").Value);
            Assert.Equal(_config.UserProperties.After.TelephoneNumber, userProperties.First(p => p.Name == "telephoneNumber").Value);
        }

        [Fact]
        public void Get_CreateUser_Ok()
        {
            var login = $"TestUser_{Guid.NewGuid():N}"[..12];

            Assert.False(_connector.IsUserExists(login));

            var props = new List<UserProperty>
            {
                new("firstName", "FirstName100"),
                new("lastName", ""),
                new("middleName", ""),
                new("telephoneNumber", ""),
                new("isLead", ""),
            };

            var user = new UserToCreate(login, _config.CreateUser.Password, props);

            _connector.CreateUser(user);

            Assert.True(_connector.IsUserExists(login));
        }
    }
}
