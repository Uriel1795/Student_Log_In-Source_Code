using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace loginproto
{
    public class UpdateInfoModel
    {
        [JsonPropertyName("version")]
        public required string Version { get; set; }

        [JsonPropertyName("url")]
        public required string Url { get; set; }

    }
}
