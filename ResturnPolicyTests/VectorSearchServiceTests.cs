using ReturnPolicy.Services;
using Xunit;

namespace ReturnPolicy.Tests;

public class VectorSearchServiceTests
{
    [Fact]
    public void FindMostRelevantChunk_ReturnsChunkWithHighestSimilarity()
    {
        // Arrange
        var searchService = new VectorSearchService();

        var storedEmbeddings = new List<ChunkEmbedding>
        {
            new("Returns are accepted within 30 days.", new float[] { 0.9f, 0.1f, 0.0f }),
            new("Items must be washed before shipping back.", new float[] { 0.1f, 0.9f, 0.0f })
        };

        // A query vector that closely aligns with the first chunk
        ReadOnlyMemory<float> queryVector = new float[] { 0.85f, 0.15f, 0.0f };

        // Act
        var bestMatch = searchService.FindMostRelevantChunk(queryVector, storedEmbeddings);

        // Assert
        Assert.NotNull(bestMatch);
        Assert.Equal("Returns are accepted within 30 days.", bestMatch);
    }
}