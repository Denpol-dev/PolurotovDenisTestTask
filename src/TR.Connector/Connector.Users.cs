using System.Text;
using System.Text.Json;
using TR.Connector.ApiDto;
using TR.Connector.ApiDto.RequestDTO;
using TR.Connectors.Api.Entities;

namespace TR.Connector
{
    public partial class Connector
    {
        public bool IsUserExists(string userLogin)
        {
            var httpClient = CreateClient();

            var response = httpClient.GetAsync("api/v1/users/all").Result;
            var json = response.Content.ReadAsStringAsync().Result;

            var api = JsonSerializer.Deserialize<ApiResponse<List<UserShortDTO>>>(json)
                      ?? throw new InvalidOperationException("Пустой ответ от API (users/all).");

            var users = api.EnsureSuccess();

            return users.Any(u => u.Login.Equals(userLogin, StringComparison.OrdinalIgnoreCase));
        }

        public void CreateUser(UserToCreate user)
        {
            var httpClient = CreateClient();

            var dto = new NewUserDTO
            {
                Login = user.Login,
                Password = user.HashPassword,

                LastName = GetProp(user, "lastName"),
                FirstName = GetProp(user, "firstName"),
                MiddleName = GetProp(user, "middleName"),
                TelephoneNumber = GetProp(user, "telephoneNumber"),

                IsLead = TryGetBoolProp(user, "isLead"),

                Status = string.Empty
            };

            var content = new StringContent(JsonSerializer.Serialize(dto), UnicodeEncoding.UTF8, "application/json");
            var response = httpClient.PostAsync("api/v1/users/create", content).Result;
            var json = response.Content.ReadAsStringAsync().Result;

            var api = JsonSerializer.Deserialize<ApiResponse<object>>(json)
                      ?? throw new InvalidOperationException("Пустой ответ от API (users/create).");

            if (!api.Success)
                throw new InvalidOperationException(api.ErrorText ?? "Ошибка API при создании пользователя.");
        }

        private static string GetProp(UserToCreate user, string name)
            => user.Properties.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase))?.Value
               ?? string.Empty;

        private static bool? TryGetBoolProp(UserToCreate user, string name)
        {
            var raw = user.Properties.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase))?.Value;

            if (string.IsNullOrWhiteSpace(raw))
                return null;

            return bool.TryParse(raw, out var b) ? b : null;
        }

        public void AddUserPermissions(string userLogin, IEnumerable<string> rightIds)
        {
            var httpClient = CreateClient();

            //проверяем что пользователь не залочен.
            var user = GetUserShort(userLogin);

            if (user.Status.Equals("Lock", StringComparison.OrdinalIgnoreCase))
            {
                Logger.Error($"Пользователь {userLogin} залочен.");
                return;
            }

            foreach (var rightId in rightIds)
            {
                var (kind, id) = ParsePermissionId(rightId);

                var resp = kind switch
                {
                    PermissionKind.ItRole => httpClient.PutAsync($"api/v1/users/{userLogin}/add/role/{id}", null).Result,
                    PermissionKind.RequestRight => httpClient.PutAsync($"api/v1/users/{userLogin}/add/right/{id}", null).Result,
                    _ => throw new Exception($"Тип доступа {kind} не определен")
                };

                var json = resp.Content.ReadAsStringAsync().Result;
                var api = JsonSerializer.Deserialize<ApiResponse<object>>(json);

                if (api is not null && !api.Success)
                    throw new InvalidOperationException(api.ErrorText ?? "Ошибка API при назначении прав.");
            }
        }

        public void RemoveUserPermissions(string userLogin, IEnumerable<string> rightIds)
        {
            var httpClient = CreateClient();

            //проверяем что пользователь не залочен.
            var user = GetUserShort(userLogin);

            if (user.Status.Equals("Lock", StringComparison.OrdinalIgnoreCase))
            {
                Logger.Error($"Пользователь {userLogin} залочен.");
                return;
            }

            foreach (var rightId in rightIds)
            {
                var (kind, id) = ParsePermissionId(rightId);

                var resp = kind switch
                {
                    PermissionKind.ItRole => httpClient.DeleteAsync($"api/v1/users/{userLogin}/drop/role/{id}").Result,
                    PermissionKind.RequestRight => httpClient.DeleteAsync($"api/v1/users/{userLogin}/drop/right/{id}").Result,
                    _ => throw new Exception($"Тип доступа {kind} не определен")
                };

                var json = resp.Content.ReadAsStringAsync().Result;
                var api = JsonSerializer.Deserialize<ApiResponse<object>>(json);

                if (api is not null && !api.Success)
                    throw new InvalidOperationException(api.ErrorText ?? "Ошибка API при отзыве прав.");
            }
        }

        private UserShortDTO GetUserShort(string userLogin)
        {
            var httpClient = CreateClient();

            var response = httpClient.GetAsync("api/v1/users/all").Result;
            var json = response.Content.ReadAsStringAsync().Result;

            var api = JsonSerializer.Deserialize<ApiResponse<List<UserShortDTO>>>(json)
                      ?? throw new InvalidOperationException("Пустой ответ от API (users/all).");

            var users = api.EnsureSuccess();

            return users.FirstOrDefault(u => u.Login.Equals(userLogin, StringComparison.OrdinalIgnoreCase))
                   ?? throw new InvalidOperationException($"Пользователь {userLogin} не найден.");
        }

        private enum PermissionKind { ItRole, RequestRight }

        private static (PermissionKind Kind, string Id) ParsePermissionId(string raw)
        {
            var parts = raw.Split(',', 2, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
                throw new ArgumentException($"Некорректный идентификатор права: {raw}", nameof(raw));

            return parts[0] switch
            {
                "ItRole" => (PermissionKind.ItRole, parts[1]),
                "RequestRight" => (PermissionKind.RequestRight, parts[1]),
                _ => throw new ArgumentException($"Тип доступа {parts[0]} не определен", nameof(raw))
            };
        }
    }
}
