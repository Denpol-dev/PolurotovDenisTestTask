using System.Text;
using System.Text.Json;
using TR.Connector.ApiDto;

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

            var body = new { login, password };
            var content = CreateJsonContent(body);
            var response = httpClient.PostAsync("api/v1/login", content).Result;
            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(response.Content.ReadAsStringAsync().Result);
            token = tokenResponse.data.access_token;
        }

        private void ParseConnectionString(string connectionString)
        {
            Logger.Debug("Строка подключения: " + connectionString);
            foreach (var raw in connectionString.Split(';'))
            {
                var item = raw.Trim();
                if (item.Length == 0) continue;

                if (item.StartsWith("url", StringComparison.OrdinalIgnoreCase)) url = item.Split('=')[1];
                if (item.StartsWith("login", StringComparison.OrdinalIgnoreCase)) login = item.Split('=')[1];
                if (item.StartsWith("password", StringComparison.OrdinalIgnoreCase)) password = item.Split('=')[1];
            }
        }

        private static StringContent CreateJsonContent(object body) =>
            new StringContent(JsonSerializer.Serialize(body), UnicodeEncoding.UTF8, "application/json");
    }
}
