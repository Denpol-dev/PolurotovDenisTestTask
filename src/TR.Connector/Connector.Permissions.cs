using System.Text.Json;
using TR.Connector.ApiDto;
using TR.Connectors.Api.Entities;

namespace TR.Connector
{
    public partial class Connector
    {
        public IEnumerable<Permission> GetAllPermissions()
        {
            var httpClient = CreateClient();

            //Получаем ИТРоли
            var rolesJson = httpClient.GetAsync("api/v1/roles/all").Result.Content.ReadAsStringAsync().Result;
            var rolesApi = JsonSerializer.Deserialize<ApiResponse<List<RoleDTO>>>(rolesJson)
                          ?? throw new InvalidOperationException("Пустой ответ от API (roles/all).");

            var roles = rolesApi.EnsureSuccess();
            var itRolePermissions = roles.Select(r =>
                new Permission($"ItRole,{r.Id}", r.Name, r.CorporatePhoneNumber ?? string.Empty));

            //Получаем права
            var rightsJson = httpClient.GetAsync("api/v1/rights/all").Result.Content.ReadAsStringAsync().Result;
            var rightsApi = JsonSerializer.Deserialize<ApiResponse<List<RightDTO>>>(rightsJson)
                           ?? throw new InvalidOperationException("Пустой ответ от API (rights/all).");

            var rights = rightsApi.EnsureSuccess();
            var rightPermissions = rights.Select(r =>
                new Permission($"RequestRight,{r.Id}", r.Name, string.Empty));

            return itRolePermissions.Concat(rightPermissions);
        }

        public IEnumerable<string> GetUserPermissions(string userLogin)
        {
            var httpClient = CreateClient();

            //Получаем ИТРоли
            var rolesJson = httpClient.GetAsync($"api/v1/users/{userLogin}/roles").Result.Content.ReadAsStringAsync().Result;
            var rolesApi = JsonSerializer.Deserialize<ApiResponse<List<RoleDTO>>>(rolesJson)
                          ?? throw new InvalidOperationException("Пустой ответ от API (users/{login}/roles).");

            var roles = rolesApi.EnsureSuccess();
            var result1 = roles.Select(r => $"ItRole,{r.Id}");

            //Получаем права
            var rightsJson = httpClient.GetAsync($"api/v1/users/{userLogin}/rights").Result.Content.ReadAsStringAsync().Result;
            var rightsApi = JsonSerializer.Deserialize<ApiResponse<List<RightDTO>>>(rightsJson)
                           ?? throw new InvalidOperationException("Пустой ответ от API (users/{login}/rights).");

            var rights = rightsApi.EnsureSuccess();
            var result2 = rights.Select(r => $"RequestRight,{r.Id}");

            return result1.Concat(result2).ToList();
        }
    }
}
