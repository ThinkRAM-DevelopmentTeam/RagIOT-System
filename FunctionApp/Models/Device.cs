using Newtonsoft.Json;
using System;

namespace IotRagFunctionApp.Models
{
    public class Device
    {
        [JsonProperty("id")]
        public string Id { get; set; } = default!;

        [JsonProperty("deviceId")]
        public string DeviceId { get; set; } = default!;

        [JsonProperty("status")]
        public string Status { get; set; } = "operational";

        [JsonProperty("lastAlertReason")]
        public string? LastAlertReason { get; set; }

        [JsonProperty("lastAlertTime")]
        public DateTime? LastAlertTime { get; set; }

        [JsonProperty("_type")]
        public string Type { get; } = "device";
    }
}
