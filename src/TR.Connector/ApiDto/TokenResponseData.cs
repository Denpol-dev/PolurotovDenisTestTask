using System.Text.Json.Serialization;

namespace TR.Connector.ApiDto
{
    public sealed class TokenResponseData
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; init; } = string.Empty;

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; init; }
    }
}
