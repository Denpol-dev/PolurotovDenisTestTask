using System.Text.Json.Serialization;

namespace TR.Connector.ApiDto
{
    public sealed class RightDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; init; }

        [JsonPropertyName("name")]
        public string Name { get; init; } = string.Empty;

        [JsonPropertyName("users")]
        public object? Users { get; init; }
    }
}
