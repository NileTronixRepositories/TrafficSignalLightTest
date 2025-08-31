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

        [JsonProperty("T")]
        public int T { get; set; }

        [JsonProperty("L2")]
        public string L2 { get; set; }

        //[JsonPropertyName("timestamp")]
        //public DateTime Timestamp { get; set; }

        //[JsonPropertyName("status")]
        //public string Status { get; set; }

        //[JsonPropertyName("method")]
        //public string Method { get; set; }

        //[JsonPropertyName("client_ip")]
        //public string ClientIp { get; set; }

        //[JsonPropertyName("content_type")]
        //public string ContentType { get; set; }

        //[JsonPropertyName("raw")]
        //public string Raw { get; set; }

        //[JsonPropertyName("parsed")]
        //public ParsedData Parsed { get; set; }
    }

    public class ParsedData
    {
        [JsonPropertyName("L1")]
        public string L1 { get; set; }

        [JsonPropertyName("T1")]
        public int T1 { get; set; }
    }
}