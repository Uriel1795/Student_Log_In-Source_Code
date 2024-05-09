using System.Text.Json.Serialization;

namespace loginproto.Models
{
    public class UpdateInfoModel
    {
        [JsonPropertyName("version")]
        public required string Version { get; set; }

        [JsonPropertyName("url")]
        public required string Url { get; set; }

    }
}
