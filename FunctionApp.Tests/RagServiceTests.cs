using System.Collections.Generic;
using System.Threading.Tasks;
using IotRagFunctionApp.Models;
using IotRagFunctionApp.Services;
using Xunit;

namespace FunctionApp.Tests
{
    class FakeCosmosService : ICosmosService
    {
        public Task InitializeAsync() => Task.CompletedTask;
        public Task UpsertAlertAsync(Alert alert) => Task.CompletedTask;
        public Task UpsertDocumentAsync(Document document) => Task.CompletedTask;
        public Task UpsertDeviceAsync(Device device) => Task.CompletedTask;
        public Task UpsertTelemetryAsync(DeviceTelemetry telemetry) => Task.CompletedTask;

        public Task<Device> GetDeviceAsync(string deviceId)
        {
            return Task.FromResult(new Device { Id = deviceId, DeviceId = deviceId, Status = "operational" });
        }

        public Task<List<DeviceTelemetry>> GetRecentTelemetryAsync(string deviceId, int minutes = 60)
        {
            var t = new DeviceTelemetry { DeviceId = deviceId, TemperatureC = 75.5, VibrationMm = 0.4, IsAnomaly = false };
            return Task.FromResult(new List<DeviceTelemetry> { t });
        }

        public Task<List<Document>> VectorSearchAsync(List<float> embedding, int topK = 5)
        {
            var doc = new Document { Id = "d1", DeviceType = "pump", ChunkText = "Check lubrication and bearings.", SourceDoc = "pump-maintenance-sop.txt", ChunkIndex = 1 };
            return Task.FromResult(new List<Document> { doc });
        }
    }

    class FakeEmbeddingService : IEmbeddingService
    {
        public Task<List<float>> GenerateEmbeddingAsync(string text)
        {
            var v = new List<float>(new float[1536]);
            v[0] = 1f;
            return Task.FromResult(v);
        }
    }

    public class RagServiceTests
    {
        [Fact]
        public async Task QueryWithRag_ReturnsSourcesAndTelemetry()
        {
            var cosmos = new FakeCosmosService();
            var embedding = new FakeEmbeddingService();
            var rag = new RagService(cosmos, embedding);

            var response = await rag.QueryWithRagAsync("pump-001", "Why is pump overheating?");

            Assert.NotNull(response);
            Assert.False(string.IsNullOrEmpty(response.Answer));
            Assert.NotEmpty(response.Sources);
            Assert.Contains("pump-maintenance-sop.txt", response.Sources[0]);
            Assert.NotNull(response.RecentTelemetry);
        }
    }
}
