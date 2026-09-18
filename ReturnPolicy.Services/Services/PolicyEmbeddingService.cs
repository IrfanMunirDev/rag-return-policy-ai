using Microsoft.Extensions.AI;

namespace ReturnPolicy.Services;

public record ChunkEmbedding(string Chunk, ReadOnlyMemory<float> Vector);

//its a helper servce for PolicyRagEmbeddingService
public class PolicyEmbeddingService
{
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;

    public PolicyEmbeddingService(IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator)
    {
        _embeddingGenerator = embeddingGenerator;
    }

    public async Task<List<ChunkEmbedding>> GenerateEmbeddingsForChunksAsync(List<string> chunks)
    {
        if (chunks == null || chunks.Count == 0)
        {
            return new List<ChunkEmbedding>();
        }

        // Batch generate embeddings using M.E.AI abstraction
        GeneratedEmbeddings<Embedding<float>> embeddings = await _embeddingGenerator.GenerateAsync(chunks);

        var result = new List<ChunkEmbedding>();
        for (int i = 0; i < chunks.Count; i++)
        {
            result.Add(new ChunkEmbedding(chunks[i], embeddings[i].Vector));
        }

        return result;
    }
}