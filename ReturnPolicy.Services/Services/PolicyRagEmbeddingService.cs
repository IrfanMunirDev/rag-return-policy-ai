using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using ReturnPolicy.Infrastructure.Data;
using ReturnPolicy.Infrastructure.Entities;

namespace ReturnPolicy.Services;

public class PolicyRagEmbeddingService
{
    private readonly IChatClient _chatClient;
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;
    private readonly PolicyChunker _chunker;
    private readonly VectorSearchService _vectorSearch;
    private readonly PolicyDbContext _dbContext;
    private readonly string _policyPath;

    public PolicyRagEmbeddingService(IChatClient chatClient,
                                     IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
                                     PolicyChunker chunker,
                                     VectorSearchService vectorSearch,
                                     PolicyDbContext dbContext)
    {
        _chatClient = chatClient;
        _embeddingGenerator = embeddingGenerator;
        _chunker = chunker;
        _vectorSearch = vectorSearch;
        _dbContext = dbContext;

        // Use AppContext.BaseDirectory to safely locate the Data folder across any project type
        var baseDir = AppContext.BaseDirectory;
        var dataDir = Path.Combine(baseDir, "Data");

        if (!Directory.Exists(dataDir))
        {
            Directory.CreateDirectory(dataDir);
        }

        _policyPath = Path.Combine(dataDir, "return_policy.txt");
    }

    private async Task EnsurePolicyLoadedAsync()
    {
        // Ensure EF Core database and tables are created
        await _dbContext.Database.EnsureCreatedAsync();

        // If chunks already exist in SQLite, skip seeding
        if (await _dbContext.PolicyChunks.AnyAsync()) return;

        if (!File.Exists(_policyPath)) return;

        var policyText = await File.ReadAllTextAsync(_policyPath);
        var chunks = _chunker.ChunkText(policyText, maxChunkLength: 400);

        var embeddingService = new PolicyEmbeddingService(_embeddingGenerator);
        var embeddedChunks = await embeddingService.GenerateEmbeddingsForChunksAsync(chunks);

        foreach (var item in embeddedChunks)
        {
            _dbContext.PolicyChunks.Add(new PolicyChunkEntity
            {
                Text = item.Chunk,
                Embedding = FloatArrayToBytes(item.Vector.ToArray())
            });
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task<string> AnswerQuestionAsync(string userQuestion)
    {
        await EnsurePolicyLoadedAsync();

        // 1. Convert user's question into a query vector using Ollama
        var queryEmbedding = await _embeddingGenerator.GenerateAsync(userQuestion);
        var queryVector = queryEmbedding.Vector;

        // 2. Load stored vectors from EF Core database
        var entities = await _dbContext.PolicyChunks.ToListAsync();
        var storedEmbeddings = entities
            .Select(e => new ChunkEmbedding(e.Text, BytesToFloatArray(e.Embedding)))
            .ToList();

        // 3. Find the most relevant chunk using vector search
        var relevantChunk = _vectorSearch.FindMostRelevantChunk(queryVector, storedEmbeddings);

        // 4. Formulate prompt for local phi4-mini model
        var systemPrompt = "You are a helpful customer service assistant. Answer accurately using ONLY the provided context snippet below. If unknown, state that you don't know.";
        var userPrompt = $"Context: {relevantChunk}\n\nQuestion: {userQuestion}";

        var response = await _chatClient.GetResponseAsync([
            new ChatMessage(ChatRole.System, systemPrompt),
            new ChatMessage(ChatRole.User, userPrompt)
        ]);

        return response.Text ?? "No response generated.";
    }

    private static byte[] FloatArrayToBytes(float[] array)
    {
        var bytes = new byte[array.Length * sizeof(float)];
        Buffer.BlockCopy(array, 0, bytes, 0, bytes.Length);
        return bytes;
    }

    private static float[] BytesToFloatArray(byte[] bytes)
    {
        var array = new float[bytes.Length / sizeof(float)];
        Buffer.BlockCopy(bytes, 0, array, 0, bytes.Length);
        return array;
    }
}