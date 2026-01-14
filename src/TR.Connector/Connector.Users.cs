using System.Text;
using System.Text.Json;
using TR.Connector.ApiDto;
using TR.Connectors.Api.Entities;

namespace TR.Connector
{
    public partial class Connector
    {
        public bool IsUserExists(string userLogin)
        {
            var httpClient = CreateClient();

            var response = httpClient.GetAsync($"api/v1/users/all").Result;
            var userResponse = JsonSerializer.Deserialize<UserResponse>(response.Content.ReadAsStringAsync().Result);
            var user = userResponse.data.FirstOrDefault(_ => _.login == userLogin);

            if (user != null) return true;

            return false;
        }

        public void CreateUser(UserToCreate user)
        {
            var httpClient = CreateClient();

            var newUser = new CreateUSerDTO()
            {
                login = user.Login,
                password = user.HashPassword,

                lastName = user.Properties.FirstOrDefault(p => p.Name.Equals("lastName", StringComparison.OrdinalIgnoreCase))?.Value ?? string.Empty,
                firstName = user.Properties.FirstOrDefault(p => p.Name.Equals("firstName", StringComparison.OrdinalIgnoreCase))?.Value ?? string.Empty,
                middleName = user.Properties.FirstOrDefault(p => p.Name.Equals("middleName", StringComparison.OrdinalIgnoreCase))?.Value ?? string.Empty,

                telephoneNumber = user.Properties.FirstOrDefault(p => p.Name.Equals("telephoneNumber", StringComparison.OrdinalIgnoreCase))?.Value ?? string.Empty,
                isLead = bool.TryParse(user.Properties.FirstOrDefault(p => p.Name.Equals("isLead", StringComparison.OrdinalIgnoreCase))?.Value ?? string.Empty, out bool isLeadValue)
                    ? isLeadValue
                    : false,

                status = string.Empty
            };

            var content = new StringContent(JsonSerializer.Serialize(newUser), UnicodeEncoding.UTF8, "application/json");
            httpClient.PostAsync("api/v1/users/create", content).Wait();
        }

        public void AddUserPermissions(string userLogin, IEnumerable<string> rightIds)
        {
            var httpClient = CreateClient();

            //проверяем что пользователь не залочен.
            var response = httpClient.GetAsync($"api/v1/users/all").Result;
            var userResponse = JsonSerializer.Deserialize<UserResponse>(response.Content.ReadAsStringAsync().Result);
            var user = userResponse.data.FirstOrDefault(_ => _.login == userLogin);

            if (user != null && user.status == "Lock")
            {
                Logger.Error($"Пользователь {userLogin} залочен.");
                return;
            }
            //Назначаем права.
            else if (user != null && user.status == "Unlock")
            {
                foreach (var rightId in rightIds)
                {
                    var rightStr = rightId.Split(',');
                    switch (rightStr[0])
                    {
                        case "ItRole":
                            httpClient.PutAsync($"api/v1/users/{userLogin}/add/role/{rightStr[1]}", null).Wait();
                            break;
                        case "RequestRight":
                            httpClient.PutAsync($"api/v1/users/{userLogin}/add/right/{rightStr[1]}", null).Wait();
                            break;
                        default:
                            throw new Exception($"Тип доступа {rightStr[0]} не определен");
                    }
                }
            }
        }

        public void RemoveUserPermissions(string userLogin, IEnumerable<string> rightIds)
        {
            var httpClient = CreateClient();

            //проверяем что пользователь не залочен.
            var response = httpClient.GetAsync($"api/v1/users/all").Result;
            var userResponse = JsonSerializer.Deserialize<UserResponse>(response.Content.ReadAsStringAsync().Result);
            var user = userResponse.data.FirstOrDefault(_ => _.login == userLogin);

            if (user != null && user.status == "Lock")
            {
                Logger.Error($"Пользователь {userLogin} залочен.");
                return;
            }
            //отзываем права.
            else if (user != null && user.status == "Unlock")
            {
                foreach (var rightId in rightIds)
                {
                    var rightStr = rightId.Split(',');
                    switch (rightStr[0])
                    {
                        case "ItRole":
                            httpClient.DeleteAsync($"api/v1/users/{userLogin}/drop/role/{rightStr[1]}").Wait();
                            break;
                        case "RequestRight":
                            httpClient.DeleteAsync($"api/v1/users/{userLogin}/drop/right/{rightStr[1]}").Wait();
                            break;
                        default:
                            throw new Exception($"Тип доступа {rightStr[0]} не определен");
                    }
                }
            }
        }
    }
}
