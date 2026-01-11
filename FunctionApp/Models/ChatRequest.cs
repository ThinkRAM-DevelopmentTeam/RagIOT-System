using Newtonsoft.Json;
using System.Collections.Generic;

namespace IotRagFunctionApp.Models
{
    public class ChatRequest
    {
        [JsonProperty("deviceId")]
        public string DeviceId { get; set; } = default!;

        [JsonProperty("question")]
        public string Question { get; set; } = default!;
    }

    public class ChatResponse
    {
        [JsonProperty("answer")]
        public string Answer { get; set; } = default!;

        [JsonProperty("sources")]
        public List<string> Sources { get; set; } = new();

        [JsonProperty("recentTelemetry")]
        public object? RecentTelemetry { get; set; }
    }
}
