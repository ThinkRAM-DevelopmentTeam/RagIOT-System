using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using IotRagFunctionApp.Models;
using IotRagFunctionApp.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace IotRagFunctionApp.Functions
{
    public class IngestDocuments
    {
        private readonly ICosmosService _cosmos;
        private readonly IEmbeddingService _embedding;

        public IngestDocuments(ICosmosService cosmos, IEmbeddingService embedding)
        {
            _cosmos = cosmos;
            _embedding = embedding;
        }

        // TEMPORARILY DISABLED - Testing HTTP endpoint functionality
        /*
        [Function("IngestDocuments")]
        public async Task Run([BlobTrigger("manuals/{name}", Connection = "BlobStorageConnection")] BlobClient blobClient, string name, FunctionContext context)
        {
            var logger = context.GetLogger("IngestDocuments");
            try
            {
                logger.LogInformation("Processing document: {name}", name);
                var download = await blobClient.DownloadAsync();
                string content = new System.IO.StreamReader(download.Value.Content).ReadToEnd();
                var chunks = ChunkText(content, chunkSize: 500);
                string deviceType = name.Split('-')[0];
                int chunkIndex = 0;
                foreach (var chunk in chunks)
                {
                    var embedding = await _embedding.GenerateEmbeddingAsync(chunk);
                    var document = new Document
                    {
                        Id = $"{name}-{chunkIndex}",
                        DeviceType = deviceType,
                        ChunkText = chunk,
                        Embedding = embedding,
                        SourceDoc = name,
                        ChunkIndex = chunkIndex
                    };
                    await _cosmos.UpsertDocumentAsync(document);
                    chunkIndex++;
                }
                logger.LogInformation("Ingested {count} chunks from {name}", chunks.Count, name);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error ingesting document: {ex.Message}");
            }
        }

        private List<string> ChunkText(string text, int chunkSize = 500)
        {
            var chunks = new List<string>();
            var sentences = text.Split(new[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
            var currentChunk = "";
            foreach (var sentence in sentences)
            {
                if ((currentChunk.Length + sentence.Length) > chunkSize && !string.IsNullOrEmpty(currentChunk))
                {
                    chunks.Add(currentChunk.Trim());
                    currentChunk = "";
                }
                currentChunk += sentence + ".";
            }
            if (!string.IsNullOrEmpty(currentChunk)) chunks.Add(currentChunk.Trim());
            return chunks;
        }
        */
    }
}
