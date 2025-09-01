using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace TrafficSignalLight.Dto
{
    public class TrafficLightResponse
    {
        [JsonProperty("L1")]
        public string L1 { get; set; }

        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("T1")]
        public int T1 { get; set; }

        [JsonProperty("T2")]
        public int T2 { get; set; }

        [JsonProperty("L2")]
        public string L2 { get; set; }
    }

    public class ParsedData
    {
        [JsonPropertyName("L1")]
        public string L1 { get; set; }

        [JsonPropertyName("T1")]
        public int T1 { get; set; }
    }
}