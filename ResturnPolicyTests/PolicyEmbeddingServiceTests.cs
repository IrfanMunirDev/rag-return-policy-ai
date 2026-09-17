using Microsoft.Extensions.AI;
using Moq;
using ReturnPolicy.Services;
using Xunit;

namespace ReturnPolicy.Tests;

public class PolicyEmbeddingServiceTests
{
    [Fact]
    public async Task GenerateEmbeddingsAsync_GivenChunks_ReturnsChunksWithVectors()
    {
        // Arrange
        var chunks = new List<string> { "30-day return window applies.", "Items must be unused.", "Items must not be unused." };

        // Create a mock IEmbeddingGenerator so the test runs instantly in memory without Ollama
        var mockGenerator = new Mock<IEmbeddingGenerator<string, Embedding<float>>>();

        // Setup mock to return a dummy vector (e.g., 3 dimensions) for each input chunk
        //Note: moq.it isn't a separate entity or site; it refers to the It class inside the Moq library—such as It.IsAny<T>() or It.Is<T>()—which is a matching helper used to specify conditions for method arguments.
        mockGenerator
            .Setup(g => g.GenerateAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<EmbeddingGenerationOptions?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GeneratedEmbeddings<Embedding<float>>([
               new Embedding<float>(new float[] { 0.1f, 0.2f, 0.3f }),
                new Embedding<float>(new float[] { 0.4f, 0.5f, 0.6f }),
                new Embedding<float>(new float[] { 0.4f, 0.5f, 0.6f })
            ]));

        var service = new PolicyEmbeddingService(mockGenerator.Object);

        // Act
        var results = await service.GenerateEmbeddingsForChunksAsync(chunks);

        // Assert
        Assert.Equal(3, results.Count);
        Assert.Equal("30-day return window applies.", results[0].Chunk);
        Assert.Equal(3, results[0].Vector.Length);
        Assert.Equal(0.1f, results[0].Vector.Span[0]);

    }
}