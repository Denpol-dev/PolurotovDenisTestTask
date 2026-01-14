using System.Text;
using System.Text.Json;
using TR.Connector.ApiDto;
using TR.Connectors.Api.Entities;

namespace TR.Connector
{
    public partial class Connector
    {
        public IEnumerable<Property> GetAllProperties()
        {
            var props = new List<Property>();
            foreach (var propertyInfo in new UserPropertyData().GetType().GetProperties())
            {
                if (propertyInfo.Name == "login") continue;

                props.Add(new Property(propertyInfo.Name, propertyInfo.Name));
            }
            return props;
        }

        public IEnumerable<UserProperty> GetUserProperties(string userLogin)
        {
            var httpClient = CreateClient();

            var response = httpClient.GetAsync($"api/v1/users/{userLogin}").Result;
            var userResponse = JsonSerializer.Deserialize<UserPropertyResponse>(response.Content.ReadAsStringAsync().Result);

            var user = userResponse.data ?? throw new NullReferenceException($"Пользователь {userLogin} не найден");

            if (user.status == "Lock")
                throw new Exception($"Невозможно получить свойства, пользователь {userLogin} залочен");

            return user.GetType().GetProperties()
                .Select(_ => new UserProperty(_.Name, _.GetValue(user) as string));
        }

        public void UpdateUserProperties(IEnumerable<UserProperty> properties, string userLogin)
        {
            var httpClient = CreateClient();

            var response = httpClient.GetAsync($"api/v1/users/{userLogin}").Result;
            var userResponse = JsonSerializer.Deserialize<UserPropertyResponse>(response.Content.ReadAsStringAsync().Result);

            var user = userResponse.data ?? throw new NullReferenceException($"Пользователь {userLogin} не найден");
            if (user.status == "Lock")
                throw new Exception($"Невозможно обновить свойства, пользователь {userLogin} залочен");

            foreach (var property in properties)
            {
                foreach (var userProp in user.GetType().GetProperties())
                {
                    if (property.Name == userProp.Name)
                    {
                        userProp.SetValue(user, property.Value);
                    }
                }
            }

            var content = new StringContent(JsonSerializer.Serialize(user), UnicodeEncoding.UTF8, "application/json");
            httpClient.PutAsync("api/v1/users/edit", content).Wait();
        }
    }
}
