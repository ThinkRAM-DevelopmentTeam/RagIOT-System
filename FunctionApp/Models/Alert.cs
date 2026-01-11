using Newtonsoft.Json;
using System;

namespace IotRagFunctionApp.Models
{
    public class Alert
    {
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [JsonProperty("deviceId")]
        public string DeviceId { get; set; } = default!;

        [JsonProperty("reason")]
        public string Reason { get; set; } = default!;

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [JsonProperty("acknowledged")]
        public bool Acknowledged { get; set; }

        [JsonProperty("_type")]
        public string Type { get; } = "alert";
    }
}
