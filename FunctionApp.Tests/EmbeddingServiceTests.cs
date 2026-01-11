using System.Threading.Tasks;
using IotRagFunctionApp.Services;
using Xunit;

namespace FunctionApp.Tests
{
    public class EmbeddingServiceTests
    {
        [Fact]
        public async Task MockEmbedding_IsDeterministic_And_CorrectLength()
        {
            var svc = new MockEmbeddingService();
            var e1 = await svc.GenerateEmbeddingAsync("hello world");
            var e2 = await svc.GenerateEmbeddingAsync("hello world");

            Assert.NotNull(e1);
            Assert.NotNull(e2);
            Assert.Equal(e1.Count, e2.Count);
            Assert.Equal(1536, e1.Count);
            Assert.Equal(e1, e2);
        }
    }
}
