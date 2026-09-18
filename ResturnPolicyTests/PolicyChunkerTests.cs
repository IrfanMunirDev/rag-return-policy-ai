using ReturnPolicy.Services;
using Xunit;

namespace ReturnPolicy.Tests;

public class PolicyChunkerTests
{
    [Fact]
    public void ChunkText_WithEmptyOrNullText_ReturnsEmptyList()
    {
        // Arrange
        var chunker = new PolicyChunker();

        // Act
        var chunks = chunker.ChunkText(string.Empty, maxChunkLength: 100);

        // Assert
        Assert.Empty(chunks);
    }

    [Fact]
    public void ChunkText_WithShortText_ReturnsSingleChunk()
    {
        // Arrange
        var chunker = new PolicyChunker();
        var text = "This is a short return policy statement.";

        // Act
        var chunks = chunker.ChunkText(text, maxChunkLength: 200);

        // Assert
        Assert.Single(chunks);
        Assert.Equal(text, chunks[0]);
    }

    [Fact]
    public void ChunkText_WithLongMultiParagraphText_SplitsIntoMultipleChunks()
    {
        // Arrange
        var chunker = new PolicyChunker();
        var paragraph1 = "Paragraph one discusses 30-day return windows.";
        var paragraph2 = "Paragraph two discusses item condition requirements.";
        var fullText = $"{paragraph1}\n\n{paragraph2}";

        // Act - set a small max length to force splitting
        var chunks = chunker.ChunkText(fullText, maxChunkLength: 50);

        // Assert
        Assert.True(chunks.Count > 1);
        Assert.Contains(paragraph1, chunks[0]);
    }
}