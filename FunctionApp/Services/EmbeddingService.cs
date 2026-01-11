using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IotRagFunctionApp.Services
{
    public interface IEmbeddingService
    {
        Task<List<float>> GenerateEmbeddingAsync(string text);
    }

    public class MockEmbeddingService : IEmbeddingService
    {
        // Simple deterministic mock: hash-based pseudo-random vector
        public Task<List<float>> GenerateEmbeddingAsync(string text)
        {
            var hash = text.GetHashCode();
            var rnd = new Random(hash);
            var embedding = Enumerable.Range(0, 1536)
                .Select(_ => (float)rnd.NextDouble())
                .ToList();
            return Task.FromResult(embedding);
        }
    }

    public class AzureOpenAIEmbeddingService : IEmbeddingService
    {
        public Task<List<float>> GenerateEmbeddingAsync(string text)
        {
            throw new NotImplementedException("Use MockEmbeddingService for local dev");
        }
    }
}
