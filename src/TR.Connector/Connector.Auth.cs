using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using TR.Connector.ApiDto;
using TR.Connector.ApiDto.RequestDTO;

namespace TR.Connector
{
    public partial class Connector
    {
        public void StartUp(string connectionString)
        {
            //Парсим строку подключения.
            ParseConnectionString(connectionString);

            //Проходим аунтификацию на сервере.
            var httpClient = CreateClient(withAuth: false);

            var body = new UIAuthConfig
            {
                Login = login,
                Password = password
            };

            var content = CreateJsonContent(body);

            var response = httpClient.PostAsync("api/v1/login", content).Result;
            var json = response.Content.ReadAsStringAsync().Result;

            var api = JsonSerializer.Deserialize<ApiResponse<TokenResponseData>>(json)
                      ?? throw new InvalidOperationException("Пустой ответ от API (login).");

            try
            {
                var tokenData = api.EnsureSuccess();
                token = tokenData.AccessToken;
            }
            catch (Exception ex)
            {
                Logger.Error($"Ошибка аутентификации: {api.ErrorText ?? ex.Message}");
                throw;
            }
        }

        private void ParseConnectionString(string connectionString)
        {
            Logger.Debug("Строка подключения: " + connectionString);

            foreach (var raw in connectionString.Split(';'))
            {
                var item = raw.Trim();
                if (item.Length == 0) continue;

                var parts = item.Split('=', 2);
                if (parts.Length != 2) continue;

                var key = parts[0].Trim();
                var value = parts[1].Trim();

                if (key.Equals("url", StringComparison.OrdinalIgnoreCase)) url = value;
                else if (key.Equals("login", StringComparison.OrdinalIgnoreCase)) login = value;
                else if (key.Equals("password", StringComparison.OrdinalIgnoreCase)) password = value;
            }
        }

        private static StringContent CreateJsonContent(object body) =>
            new StringContent(JsonSerializer.Serialize(body), UnicodeEncoding.UTF8, "application/json");
    }
}
