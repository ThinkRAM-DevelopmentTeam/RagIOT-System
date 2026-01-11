using Newtonsoft.Json;
using System;

namespace IotRagFunctionApp.Models
{
    public class DeviceTelemetry
    {
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [JsonProperty("deviceId")]
        public string DeviceId { get; set; } = default!;

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [JsonProperty("temperatureC")]
        public double TemperatureC { get; set; }

        [JsonProperty("vibrationMm")]
        public double VibrationMm { get; set; }

        [JsonProperty("isAnomaly")]
        public bool IsAnomaly { get; set; }

        [JsonProperty("_type")]
        public string Type { get; } = "telemetry";
    }
}
