using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TR.Connector.ApiDto
{
    public sealed class UserShortDTO
    {
        [JsonPropertyName("login")]
        public string Login { get; init; } = string.Empty;

        [JsonPropertyName("status")]
        public string Status { get; init; } = string.Empty;
    }
}
