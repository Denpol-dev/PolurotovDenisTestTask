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
            var response = httpClient.GetAsync("api/v1/roles/all").Result;
            var itRoleResponse = JsonSerializer.Deserialize<RoleResponse>(response.Content.ReadAsStringAsync().Result);
            var itRolePermissions =
                itRoleResponse.data.Select(_ => new Permission($"ItRole,{_.id}", _.name, _.corporatePhoneNumber));

            //Получаем права
            response = httpClient.GetAsync("api/v1/rights/all").Result;
            var RightResponse = JsonSerializer.Deserialize<RoleResponse>(response.Content.ReadAsStringAsync().Result);
            var RightPermissions = RightResponse.data.Select(_ =>
                new Permission($"RequestRight,{_.id}", _.name, _.corporatePhoneNumber));

            return itRolePermissions.Concat(RightPermissions);
        }

        public IEnumerable<string> GetUserPermissions(string userLogin)
        {
            var httpClient = CreateClient();

            //Получаем ИТРоли
            var response = httpClient.GetAsync($"api/v1/users/{userLogin}/roles").Result;
            var itRoleResponse = JsonSerializer.Deserialize<UserRoleResponse>(response.Content.ReadAsStringAsync().Result);
            var result1 = itRoleResponse.data.Select(_ => $"ItRole,{_.id}").ToList();

            //Получаем права
            response = httpClient.GetAsync($"api/v1/users/{userLogin}/rights").Result;
            var RightResponse = JsonSerializer.Deserialize<UserRoleResponse>(response.Content.ReadAsStringAsync().Result);
            var result2 = RightResponse.data.Select(_ => $"RequestRight,{_.id}").ToList();

            return result1.Concat(result2).ToList();
        }
    }
}
