using System.Net.Http.Headers;
using TR.Connectors.Api.Interfaces;

namespace TR.Connector
{
    public partial class Connector : IConnector
    {
        public ILogger Logger { get; set; }

        private string url = "";
        private string login = "";
        private string password = "";

        private string token = "";

        //Пустой конструктор
        public Connector() { }

        private HttpClient CreateClient(bool withAuth = true)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(url)
            };

            if (withAuth && !string.IsNullOrEmpty(token))
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return httpClient;
        }
    }
}
