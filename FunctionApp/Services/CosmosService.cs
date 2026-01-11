using Microsoft.Azure.Cosmos;
using IotRagFunctionApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IotRagFunctionApp.Services
{
    public interface ICosmosService
    {
        Task InitializeAsync();
        Task UpsertTelemetryAsync(DeviceTelemetry telemetry);
        Task UpsertDocumentAsync(Document document);
        Task UpsertDeviceAsync(Device device);
        Task UpsertAlertAsync(Alert alert);
        Task<List<Document>> VectorSearchAsync(List<float> embedding, int topK = 5);
        Task<List<DeviceTelemetry>> GetRecentTelemetryAsync(string deviceId, int minutes = 60);
        Task<Device> GetDeviceAsync(string deviceId);
    }

    public class CosmosService : ICosmosService
    {
        private readonly CosmosClient _client;
        private readonly string _databaseId = "iotdb";
        private Database? _database;
        private Container? _documentsContainer;
        private Container? _telemetryContainer;
        private Container? _devicesContainer;
        private Container? _alertsContainer;

        public CosmosService(CosmosClient client)
        {
            _client = client;
        }

        public async Task InitializeAsync()
        {
            _database = await _client.CreateDatabaseIfNotExistsAsync(_databaseId);

            _documentsContainer = await _database.CreateContainerIfNotExistsAsync(new ContainerProperties("documents", "/deviceType")) ;
            _telemetryContainer = await _database.CreateContainerIfNotExistsAsync(new ContainerProperties("telemetry", "/deviceId"));
            _devicesContainer = await _database.CreateContainerIfNotExistsAsync(new ContainerProperties("devices", "/deviceId"));
            _alertsContainer = await _database.CreateContainerIfNotExistsAsync(new ContainerProperties("alerts", "/deviceId"));
        }

        public async Task UpsertTelemetryAsync(DeviceTelemetry telemetry)
        {
            if (_telemetryContainer == null) throw new InvalidOperationException("Cosmos not initialized");
            await _telemetryContainer.UpsertItemAsync(telemetry, new PartitionKey(telemetry.DeviceId));
        }

        public async Task UpsertDocumentAsync(Document document)
        {
            if (_documentsContainer == null) throw new InvalidOperationException("Cosmos not initialized");
            await _documentsContainer.UpsertItemAsync(document, new PartitionKey(document.DeviceType));
        }

        public async Task UpsertDeviceAsync(Device device)
        {
            if (_devicesContainer == null) throw new InvalidOperationException("Cosmos not initialized");
            await _devicesContainer.UpsertItemAsync(device, new PartitionKey(device.DeviceId));
        }

        public async Task UpsertAlertAsync(Alert alert)
        {
            if (_alertsContainer == null) throw new InvalidOperationException("Cosmos not initialized");
            await _alertsContainer.UpsertItemAsync(alert, new PartitionKey(alert.DeviceId));
        }

        public async Task<List<Document>> VectorSearchAsync(List<float> embedding, int topK = 5)
        {
            if (_documentsContainer == null) throw new InvalidOperationException("Cosmos not initialized");

            // Fallback simple similarity search (cosine on stored embeddings) performed client-side if vector query not supported
            var sql = "SELECT * FROM c";
            var iterator = _documentsContainer.GetItemQueryIterator<Document>(sql);
            var all = new List<Document>();
            while (iterator.HasMoreResults)
            {
                var feed = await iterator.ReadNextAsync();
                all.AddRange(feed.Resource);
            }

            // compute cosine similarity client side
            double Dot(List<float> a, List<float> b)
            {
                double sum = 0;
                for (int i = 0; i < Math.Min(a.Count, b.Count); i++) sum += a[i] * b[i];
                return sum;
            }
            double Norm(List<float> v) => Math.Sqrt(v.Select(x => x * x).Sum());

            var scored = all.Select(d => new { Doc = d, Score = (float)(Dot(d.Embedding, embedding) / (Norm(d.Embedding) * Norm(embedding) + 1e-8)) })
                            .OrderByDescending(x => x.Score)
                            .Take(topK)
                            .Select(x => x.Doc)
                            .ToList();

            return scored;
        }

        public async Task<List<DeviceTelemetry>> GetRecentTelemetryAsync(string deviceId, int minutes = 60)
        {
            if (_telemetryContainer == null) throw new InvalidOperationException("Cosmos not initialized");
            var cutoff = DateTime.UtcNow.AddMinutes(-minutes);
            var query = new QueryDefinition("SELECT * FROM c WHERE c.deviceId = @deviceId AND c.timestamp > @cutoff ORDER BY c.timestamp DESC")
                        .WithParameter("@deviceId", deviceId)
                        .WithParameter("@cutoff", cutoff);

            var iterator = _telemetryContainer.GetItemQueryIterator<DeviceTelemetry>(query);
            var results = new List<DeviceTelemetry>();
            while (iterator.HasMoreResults)
            {
                var feed = await iterator.ReadNextAsync();
                results.AddRange(feed.Resource);
            }
            return results;
        }

        public async Task<Device> GetDeviceAsync(string deviceId)
        {
            if (_devicesContainer == null) throw new InvalidOperationException("Cosmos not initialized");
            try
            {
                var res = await _devicesContainer.ReadItemAsync<Device>(deviceId, new PartitionKey(deviceId));
                return res.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new Device { Id = deviceId, DeviceId = deviceId, Status = "operational" };
            }
        }
    }
}
