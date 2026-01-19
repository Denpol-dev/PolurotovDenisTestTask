using System.Text.Json.Serialization;

namespace TR.Connector.ApiDto.RequestDTO;

internal sealed class UIAuthConfig
{
    [JsonPropertyName("login")]
    public string? Login { get; set; }

    [JsonPropertyName("password")]
    public string? Password { get; set; }
}
