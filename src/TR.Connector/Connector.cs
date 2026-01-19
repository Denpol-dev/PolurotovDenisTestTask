using System.Net.Http.Headers;
using TR.Connectors.Api.Interfaces;

namespace TR.Connector
{
    public partial class Connector : IConnector
    {
        public ILogger Logger { get; set; } = new NullLogger();

        private string url = "";
        private string login = "";
        private string password = "";

        private string token = "";

        //Пустой конструктор
        public Connector() { }

        private HttpClient CreateClient(bool withAuth = true)
        {
            EnsureInitialized();

            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(url)
            };

            if (withAuth)
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return httpClient;
        }

        private void EnsureInitialized()
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new InvalidOperationException(
                    "Коннектор не инициализирован. Необходимо вызвать метод StartUp перед использованием.");

            if (string.IsNullOrWhiteSpace(token))
                throw new InvalidOperationException(
                    "Отсутствует токен аутентификации. Метод StartUp завершился неуспешно.");
        }
    }
}
