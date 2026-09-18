using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using OpenAI;
using ReturnPolicy.Infrastructure.Data;
using ReturnPolicy.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Register EF Core SQLite
builder.Services.AddDbContext<PolicyDbContext>(options =>
    options.UseSqlite("Data Source=Data/policy.db"));

// Configure Ollama AI Provider
var endpoint = builder.Configuration["Ollama:Endpoint"] ?? "http://localhost:11434/v1/";
var chatModel = builder.Configuration["Ollama:ChatModel"] ?? "phi4-mini";
var embedModel = builder.Configuration["Ollama:EmbedModel"] ?? "nomic-embed-text";

var ollamaClient = new OpenAIClient(
    new System.ClientModel.ApiKeyCredential("ollama"),
    new OpenAIClientOptions { Endpoint = new Uri(endpoint) }
);

builder.Services.AddChatClient(_ => ollamaClient.GetChatClient(chatModel).AsIChatClient());
builder.Services.AddEmbeddingGenerator(_ => ollamaClient.GetEmbeddingClient(embedModel).AsIEmbeddingGenerator());

// Register RAG services from ReturnPolicy.Services library
builder.Services.AddSingleton<PolicyChunker>();
builder.Services.AddSingleton<PolicyEmbeddingService>();
builder.Services.AddSingleton<VectorSearchService>();
builder.Services.AddScoped<PolicyRagPromptStuffingService>();
builder.Services.AddScoped<PolicyRagEmbeddingService>();

var app = builder.Build();

app.UseStaticFiles();
app.MapControllers();

app.Run();