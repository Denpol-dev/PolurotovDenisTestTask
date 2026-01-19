using System.Text;
using System.Text.Json;
using TR.Connector.ApiDto;
using TR.Connector.ApiDto.RequestDTO;
using TR.Connectors.Api.Entities;

namespace TR.Connector
{
    public partial class Connector
    {
        public IEnumerable<Property> GetAllProperties()
        {
            return typeof(UserByPropertiesDTO)
                .GetProperties()
                .Where(p => !p.Name.Equals("Login", StringComparison.OrdinalIgnoreCase))
                .Select(p => new Property(ToApiName(p.Name), ToApiName(p.Name)))
                .ToList();
        }

        public IEnumerable<UserProperty> GetUserProperties(string userLogin)
        {
            var httpClient = CreateClient();

            var response = httpClient.GetAsync($"api/v1/users/{userLogin}").Result;
            var json = response.Content.ReadAsStringAsync().Result;

            var api = JsonSerializer.Deserialize<ApiResponse<UserByPropertiesDTO>>(json)
                      ?? throw new InvalidOperationException("Пустой ответ от API (users/{login}).");

            var user = api.EnsureSuccess();

            if (string.Equals(user.Status, "Lock", StringComparison.OrdinalIgnoreCase))
                throw new Exception($"Невозможно получить свойства, пользователь {userLogin} залочен");

            return typeof(UserByPropertiesDTO)
                .GetProperties()
                .Select(p =>
                {
                    var apiName = ToApiName(p.Name);
                    var value = p.GetValue(user);
                    return new UserProperty(apiName, value?.ToString() ?? string.Empty);
                })
                .ToList();
        }

        public void UpdateUserProperties(IEnumerable<UserProperty> properties, string userLogin)
        {
            var httpClient = CreateClient();

            var response = httpClient.GetAsync($"api/v1/users/{userLogin}").Result;
            var json = response.Content.ReadAsStringAsync().Result;

            var api = JsonSerializer.Deserialize<ApiResponse<UserByPropertiesDTO>>(json)
                      ?? throw new InvalidOperationException("Пустой ответ от API (users/{login}).");

            var user = api.EnsureSuccess();

            if (string.Equals(user.Status, "Lock", StringComparison.OrdinalIgnoreCase))
                throw new Exception($"Невозможно обновить свойства, пользователь {userLogin} залочен");

            foreach (var prop in properties)
            {
                var targetProp = typeof(UserByPropertiesDTO).GetProperties()
                    .FirstOrDefault(p => ToApiName(p.Name).Equals(prop.Name, StringComparison.OrdinalIgnoreCase));

                if (targetProp is null)
                    continue;

                if (targetProp.PropertyType == typeof(bool?) || targetProp.PropertyType == typeof(bool))
                {
                    if (bool.TryParse(prop.Value, out var b))
                        targetProp.SetValue(user, b);
                    else
                        targetProp.SetValue(user, null);
                }
                else
                {
                    targetProp.SetValue(user, prop.Value);
                }
            }

            var content = new StringContent(JsonSerializer.Serialize(user), UnicodeEncoding.UTF8, "application/json");
            var putResp = httpClient.PutAsync("api/v1/users/edit", content).Result;
            var putJson = putResp.Content.ReadAsStringAsync().Result;

            var putApi = JsonSerializer.Deserialize<ApiResponse<object>>(putJson)
                         ?? throw new InvalidOperationException("Пустой ответ от API (users/edit).");

            if (!putApi.Success)
                throw new InvalidOperationException(putApi.ErrorText ?? "Ошибка API при обновлении пользователя.");
        }

        private static string ToApiName(string propName)
        {
            if (string.IsNullOrEmpty(propName))
                return propName;

            if (propName.Length == 1)
                return propName.ToLowerInvariant();

            return char.ToLowerInvariant(propName[0]) + propName[1..];
        }
    }
}
