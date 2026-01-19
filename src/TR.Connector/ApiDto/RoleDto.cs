using System.Text.Json.Serialization;

namespace TR.Connector.ApiDto
{
    public sealed class RoleDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; init; }

        [JsonPropertyName("name")]
        public string Name { get; init; } = string.Empty;

        [JsonPropertyName("corporatePhoneNumber")]
        public string? CorporatePhoneNumber { get; init; }
    }
}
