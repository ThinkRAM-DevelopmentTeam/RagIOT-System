using Newtonsoft.Json;
using System.Collections.Generic;

namespace IotRagFunctionApp.Models
{
    public class Document
    {
        [JsonProperty("id")]
        public string Id { get; set; } = default!;

        [JsonProperty("deviceType")]
        public string DeviceType { get; set; } = default!;

        [JsonProperty("chunkText")]
        public string ChunkText { get; set; } = default!;

        [JsonProperty("embedding")]
        public List<float> Embedding { get; set; } = new();

        [JsonProperty("sourceDoc")]
        public string SourceDoc { get; set; } = default!;

        [JsonProperty("chunkIndex")]
        public int ChunkIndex { get; set; }

        [JsonProperty("_type")]
        public string Type { get; } = "document";
    }
}
